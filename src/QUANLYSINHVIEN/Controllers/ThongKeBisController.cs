using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QUANLYSINHVIEN.Data;
using QUANLYSINHVIEN.Filters;
using QUANLYSINHVIEN.Models;
using Microsoft.AspNetCore.Http;

namespace QUANLYSINHVIEN.Controllers
{
    [Authentication]
    public class ThongKeBisController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public ThongKeBisController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: ThongKeBis
        // [CẬP NHẬT] Thêm tham số maKhoa, maLop để nhận dữ liệu từ bộ lọc
        public async Task<IActionResult> Index(string maKhoa, string maLop)
        {
            // 1. Lấy thông tin người dùng từ Session
            var role = HttpContext.Session.GetString("Role");
            var maSv = HttpContext.Session.GetString("MaSv");

            // 2. Chuẩn bị truy vấn dữ liệu gốc
            // [CẬP NHẬT] Include thêm MaLopNavigation và MaKhoaNavigation để phục vụ lọc
            var query = _context.BangDiems
                                .Include(b => b.MaSvNavigation)
                                    .ThenInclude(s => s.MaLopNavigation)
                                    .ThenInclude(l => l.MaKhoaNavigation)
                                .Include(b => b.MaMhNavigation)
                                .AsQueryable();

            // =========================================================
            // PHẦN A: LOGIC CHO SINH VIÊN (CÁ NHÂN HÓA + AI ASSISTANT)
            // =========================================================
            if (role == "SinhVien" && !string.IsNullOrEmpty(maSv))
            {
                query = query.Where(b => b.MaSv == maSv);
                ViewBag.TitleHeader = "TRỢ LÝ HỌC TẬP CÁ NHÂN";

                try
                {
                    var svInfo = await _context.SinhViens
                        .Include(s => s.MaLopNavigation)
                        .FirstOrDefaultAsync(s => s.MaSv == maSv);

                    double gpaHienTai = svInfo?.DiemTrungBinh ?? 0;
                    // Giả lập xu hướng
                    double gpaKyTruoc = gpaHienTai < 5 ? (gpaHienTai + 0.5) : (gpaHienTai - 0.3);

                    ViewBag.XuHuong = Math.Round(gpaHienTai - gpaKyTruoc, 2);
                    ViewBag.GpaHienTai = Math.Round(gpaHienTai, 2);

                    // AI Dự báo text
                    if (gpaHienTai < 4.0) ViewBag.DuBao = "Nguy cơ Rớt";
                    else if (gpaHienTai < 7.0) ViewBag.DuBao = "An toàn";
                    else ViewBag.DuBao = "Xuất sắc";

                    // Logic So sánh
                    double diemTB_Lop = 6.0; // Mặc định
                    if (svInfo != null)
                    {
                        var listDiemLop = await _context.BangDiems
                            .Where(b => b.MaSvNavigation.MaLop == svInfo.MaLop && b.DiemTb != null)
                            .Select(b => b.DiemTb)
                            .ToListAsync();

                        if (listDiemLop.Any()) diemTB_Lop = listDiemLop.Average() ?? 0;

                        if (gpaHienTai >= diemTB_Lop)
                        {
                            ViewBag.MotivationTitle = "Làm tốt lắm! 🌟";
                            ViewBag.MotivationMsg = $"Bạn đang cao hơn TB lớp ({Math.Round(diemTB_Lop, 2)}).";
                            ViewBag.CardBorder = "border-left-success";
                            ViewBag.MsgColor = "text-success";
                        }
                        else
                        {
                            ViewBag.MotivationTitle = "Cần cố gắng! ⚠️";
                            ViewBag.MotivationMsg = $"Bạn đang thấp hơn TB lớp ({Math.Round(diemTB_Lop, 2)}).";
                            ViewBag.CardBorder = "border-left-danger";
                            ViewBag.MsgColor = "text-danger";
                        }
                    }

                    // Gán dữ liệu biểu đồ so sánh
                    ViewBag.SoSanhData = JsonConvert.SerializeObject(new double[] {
                        gpaHienTai,
                        Math.Round(diemTB_Lop, 2),
                        7.5 // Điểm TB Khoa (Giả định)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // =========================================================
            // PHẦN B: LOGIC CHO ADMIN/GIẢNG VIÊN (BI DASHBOARD + DRILL DOWN)
            // =========================================================
            else
            {
                ViewBag.TitleHeader = "DASHBOARD QUẢN TRỊ & RA QUYẾT ĐỊNH";

                // --- [MỚI] LOGIC DRILL-DOWN (LỌC DỮ LIỆU) ---
                ViewBag.CurrentFilter = "Toàn trường";
                ViewBag.IsDrillDown = false;

                // 1. Lọc theo Khoa
                if (!string.IsNullOrEmpty(maKhoa))
                {
                    query = query.Where(b => b.MaSvNavigation.MaLopNavigation.MaKhoa == maKhoa);
                    ViewBag.CurrentFilter = $"Khoa: {maKhoa}";
                    ViewBag.IsDrillDown = true;
                }

                // 2. Lọc theo Lớp
                if (!string.IsNullOrEmpty(maLop))
                {
                    query = query.Where(b => b.MaSvNavigation.MaLop == maLop);
                    ViewBag.CurrentFilter = $"Lớp: {maLop}";
                    ViewBag.IsDrillDown = true;
                }

                // 3. Đổ dữ liệu cho Dropdown
                ViewBag.ListKhoa = await _context.Khoas.ToListAsync();
                if (!string.IsNullOrEmpty(maKhoa))
                {
                    // Nếu đã chọn Khoa, Dropdown Lớp chỉ hiện lớp của Khoa đó
                    ViewBag.ListLop = await _context.Lops.Where(l => l.MaKhoa == maKhoa).ToListAsync();
                }
                else
                {
                    ViewBag.ListLop = await _context.Lops.ToListAsync();
                }
                // ---------------------------------------------

                // Tính toán Insight Cards (Lưu ý: Tính trên query đã lọc)

                // Card 1: Số SV Cảnh báo (Trong phạm vi lọc)
                // Vì query là Bảng Điểm, ta cần Group theo MaSv để đếm số SV duy nhất bị yếu
                var listMaSvYeu = await query
                    .Where(b => b.MaSvNavigation.DiemTrungBinh < 5.0)
                    .Select(b => b.MaSv)
                    .Distinct()
                    .ToListAsync();
                ViewBag.SvCanhBao = listMaSvYeu.Count;

                // Card 2: Môn rớt nhiều nhất
                var monRotNhieu = await query
                    .Where(b => b.DiemTb < 4)
                    .GroupBy(b => b.MaMhNavigation.TenMh)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync();
                ViewBag.MonRotNhieu = monRotNhieu ?? "Chưa có dữ liệu";

                // Card 3: Lớp có GPA cao nhất
                var lopGioiNhat = await query
                    .GroupBy(b => b.MaSvNavigation.MaLop)
                    .OrderByDescending(g => g.Average(b => b.MaSvNavigation.DiemTrungBinh))
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync();
                ViewBag.LopGioiNhat = lopGioiNhat ?? "Chưa có dữ liệu";
            }

            // =========================================================
            // PHẦN C: DỮ LIỆU BIỂU ĐỒ CHUNG (Đã áp dụng bộ lọc Drill-down)
            // =========================================================
            var dataPoints = await query.ToListAsync();

            // Biểu đồ 1: Tỷ lệ Đậu / Rớt
            int soLuongDau = dataPoints.Count(x => x.DiemTb >= 4.0);
            int soLuongRot = dataPoints.Count(x => x.DiemTb < 4.0);
            ViewBag.TyLeDauRot = JsonConvert.SerializeObject(new List<int> { soLuongDau, soLuongRot });

            // Biểu đồ 2: Phân bố Xếp loại
            int gio = dataPoints.Count(x => x.DiemTb >= 8.0);
            int kha = dataPoints.Count(x => x.DiemTb >= 6.5 && x.DiemTb < 8.0);
            int trungBinh = dataPoints.Count(x => x.DiemTb >= 5.0 && x.DiemTb < 6.5);
            int yeu = dataPoints.Count(x => x.DiemTb < 5.0);
            ViewBag.PhanBoXepLoai = JsonConvert.SerializeObject(new List<int> { gio, kha, trungBinh, yeu });

            // Các chỉ số phụ
            ViewBag.TongSoMon = dataPoints.Count;
            ViewBag.DiemTBTichLuy = dataPoints.Any() ? Math.Round(dataPoints.Average(x => x.DiemTb ?? 0), 2) : 0;

            // [FIX LỖI BIỂU ĐỒ TRỐNG] - Đảm bảo SoSanhData không null cho trường hợp Admin
            if (ViewBag.SoSanhData == null)
            {
                ViewBag.SoSanhData = "[]";
            }

            return View(dataPoints);
        }

        // GET: ThongKeBis/AIMetrics
        public IActionResult AIMetrics()
        {
            return View();
        }
    }
}