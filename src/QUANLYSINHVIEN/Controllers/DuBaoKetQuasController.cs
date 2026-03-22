using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using QUANLYSINHVIEN.Data;
using QUANLYSINHVIEN.Filters;
using QUANLYSINHVIEN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUANLYSINHVIEN.Controllers
{
    [Authentication]
    public class DuBaoKetQuasController : Controller
    {
        private readonly WquanlysinhvienContext _context;
        private readonly MLContext _mlContext;

        public DuBaoKetQuasController(WquanlysinhvienContext context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0);
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var maSv = HttpContext.Session.GetString("MaSv");

            var query = _context.DuBaoKetQuas
                                .Include(d => d.MaSvNavigation)
                                .OrderByDescending(d => d.NgayDuBao)
                                .AsQueryable();

            if (role == "SinhVien")
            {
                if (string.IsNullOrEmpty(maSv)) return RedirectToAction("Login", "TaiKhoans");
                query = query.Where(d => d.MaSv == maSv);
            }

            return View(await query.ToListAsync());
        }

        public IActionResult TaoDuBao()
        {
            var role = HttpContext.Session.GetString("Role");
            var maSv = HttpContext.Session.GetString("MaSv");

            if (role == "SinhVien")
            {
                var myself = _context.SinhViens.Where(s => s.MaSv == maSv);
                ViewData["MaSv"] = new SelectList(myself, "MaSv", "HoTen");
            }
            else
            {
                ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChayDuBao(string[] MaSv)
        {
            // --- [BẢO MẬT] ---
            var role = HttpContext.Session.GetString("Role");
            var currentMaSv = HttpContext.Session.GetString("MaSv");

            if (role == "SinhVien")
            {
                if (string.IsNullOrEmpty(currentMaSv)) return RedirectToAction("Login", "TaiKhoans");
                MaSv = new string[] { currentMaSv };
            }

            if (MaSv == null || MaSv.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sinh viên.";
                return RedirectToAction(nameof(TaoDuBao));
            }

            // --- BƯỚC 1: LẤY DỮ LIỆU HUẤN LUYỆN (GIỮ NGUYÊN) ---
            var duLieuHoc = await _context.BangDiems
                .Include(b => b.MaSvNavigation)
                .Where(b => b.DiemQt != null && b.DiemTb != null && b.DiemTb > 0 && b.MaSvNavigation.DiemTrungBinh > 0)
                .Select(b => new StudentData
                {
                    DiemQT = (float)b.DiemQt,
                    GPA = (float)b.MaSvNavigation.DiemTrungBinh,
                    DiemTB = (float)b.DiemTb
                })
                .ToListAsync();

            if (duLieuHoc.Count < 5)
            {
                TempData["Error"] = "Dữ liệu huấn luyện chưa đủ (cần ít nhất 5 bản ghi).";
                return RedirectToAction(nameof(Index));
            }

            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(duLieuHoc);

            // --- BƯỚC 2: HUẤN LUYỆN MÔ HÌNH (CHỈ CHẠY 1 LẦN) ---
            var pipeline = _mlContext.Transforms.Concatenate("Features", "DiemQT", "GPA")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "DiemTB", maximumNumberOfIterations: 2000));

            var model = pipeline.Fit(trainingData);
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<StudentData, StudentPrediction>(model);

            // --- BƯỚC 3: TỐI ƯU HÓA LẤY DỮ LIỆU ---

            // A. Lấy thông tin Sinh viên (GPA) của những người được chọn
            var listSinhVienInfo = await _context.SinhViens
                                        .Where(s => MaSv.Contains(s.MaSv))
                                        .ToDictionaryAsync(s => s.MaSv); // Chuyển sang Dictionary để tra cứu cực nhanh

            // B. Lấy điểm QT trung bình của những người được chọn (Gom nhóm và tính TB ngay tại DB)
            var listDiemQT = await _context.BangDiems
                                    .Where(b => MaSv.Contains(b.MaSv) && b.DiemQt != null)
                                    .GroupBy(b => b.MaSv)
                                    .Select(g => new { MaSv = g.Key, DiemQtTB = g.Average(b => b.DiemQt) })
                                    .ToDictionaryAsync(x => x.MaSv, x => x.DiemQtTB);

            // C. Xóa kết quả cũ của những người này (Batch Delete)
            var oldRecords = _context.DuBaoKetQuas.Where(d => MaSv.Contains(d.MaSv));
            _context.DuBaoKetQuas.RemoveRange(oldRecords);

            // --- BƯỚC 4: DỰ BÁO TRONG BỘ NHỚ RAM (SIÊU NHANH) ---
            var listKetQuaMoi = new List<DuBaoKetQua>();
            int countSuccess = 0;

            foreach (var maSvItem in MaSv)
            {
                // Kiểm tra xem SV này có đủ dữ liệu không (tra cứu từ Dictionary đã load sẵn)
                if (!listSinhVienInfo.ContainsKey(maSvItem) || !listDiemQT.ContainsKey(maSvItem))
                    continue;

                var svInfo = listSinhVienInfo[maSvItem];
                var diemQtHienTai = listDiemQT[maSvItem];

                // Input dự báo
                var inputData = new StudentData
                {
                    DiemQT = (float)diemQtHienTai,
                    GPA = (float)svInfo.DiemTrungBinh
                };

                // AI Tính toán (Mất < 0.01ms)
                var ketQuaDuBao = predictionEngine.Predict(inputData);
                float diemDuBaoFinal = ketQuaDuBao.DiemTBDuBao;

                // --- [LOGIC BỔ TRỢ] SỬA LỖI AI ---

                // 1. Chặn biên (Clamp): Điểm không thể < 0 hoặc > 10
                diemDuBaoFinal = Math.Max(0, Math.Min(10, diemDuBaoFinal));

                // 2. Logic "Vớt": Nếu Điểm QT cao (> 8) thì Dự báo không được quá thấp (phải >= 5)
                if (inputData.DiemQT >= 8.0f && diemDuBaoFinal < 5.0f)
                {
                    diemDuBaoFinal = 5.5f; // "Vớt" lên trung bình
                }

                // 3. Logic "Phạt": Nếu Điểm QT quá thấp (< 3) thì chắc chắn Rớt (AI có báo đậu cũng ép rớt)
                if (inputData.DiemQT < 3.0f)
                {
                    diemDuBaoFinal = Math.Min(3.0f, diemDuBaoFinal);
                }

                // 4. Logic "Kéo về gần": Nếu dự báo lệch quá xa so với Điểm QT (ví dụ lệch > 4 điểm), kéo bớt lại
                if (Math.Abs(diemDuBaoFinal - inputData.DiemQT) > 4)
                {
                    // Lấy trung bình cộng giữa AI và Điểm QT để an toàn hơn
                    diemDuBaoFinal = (diemDuBaoFinal + inputData.DiemQT) / 2;
                }

                string xepLoaiDuBao = "Yếu";
                if (diemDuBaoFinal >= 8.0f) xepLoaiDuBao = "Giỏi";
                else if (diemDuBaoFinal >= 6.5f) xepLoaiDuBao = "Khá";
                else if (diemDuBaoFinal >= 5.0f) xepLoaiDuBao = "Trung bình";

                // Tạo đối tượng kết quả
                var duBaoMoi = new DuBaoKetQua
                {
                    MaSv = maSvItem,
                    NgayDuBao = DateTime.Now,
                    XacSuatTotNghiep = diemDuBaoFinal >= 5.0 ? 95 : (diemDuBaoFinal >= 4 ? 60 : 30),
                    DuBaoXepLoai = xepLoaiDuBao,
                    MoTaPhanTich = $"AI phân tích: GPA ({svInfo.DiemTrungBinh}) + Điểm QT ({diemQtHienTai:0.0}) => Dự báo thi đạt {diemDuBaoFinal:0.0}."
                };

                listKetQuaMoi.Add(duBaoMoi);
                countSuccess++;
            }

            // --- BƯỚC 5: LƯU XUỐNG DB 1 LẦN DUY NHẤT (BATCH INSERT) ---
            if (listKetQuaMoi.Count > 0)
            {
                await _context.DuBaoKetQuas.AddRangeAsync(listKetQuaMoi);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Đã chạy xong mô hình AI cho {countSuccess} sinh viên (Tốc độ cao)!";
            return RedirectToAction(nameof(Index));
        }
    }
}