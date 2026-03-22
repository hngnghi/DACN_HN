using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLYSINHVIEN.Data;
using QUANLYSINHVIEN.Filters;
using QUANLYSINHVIEN.Models;
using OfficeOpenXml;
using System.IO;

namespace QUANLYSINHVIEN.Controllers
{
    [Authentication(Roles = "Admin,GiangVien")]
    public class SinhViensController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public SinhViensController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: SinhViens
        public async Task<IActionResult> Index()
        {
            var wquanlysinhvienContext = _context.SinhViens.Include(s => s.MaLopNavigation);
            return View(await wquanlysinhvienContext.ToListAsync());
        }

        // GET: SinhViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // GET: SinhViens/Create
        public IActionResult Create()
        {
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop");
            return View();
        }

        // POST: SinhViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,NgaySinh,GioiTinh,DiaChi,Email,MaLop,DiemTrungBinh,TrangThaiHocTap")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // GET: SinhViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // POST: SinhViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,HoTen,NgaySinh,GioiTinh,DiaChi,Email,MaLop,DiemTrungBinh,TrangThaiHocTap")] SinhVien sinhVien)
        {
            if (id != sinhVien.MaSv) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // GET: SinhViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }

        // -----------------------------------------------------------
        // IMPORT EXCEL
        // -----------------------------------------------------------

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                ViewBag.Error = "Vui lòng chọn file Excel để upload.";
                return View();
            } 

            int soDongThanhCong = 0;
            int soDongLoi = 0;
            List<string> loiChiTiet = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var maSv = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var hoTen = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var ngaySinhRaw = worksheet.Cells[row, 3].Value;
                            var gioiTinh = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var diaChi = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var email = worksheet.Cells[row, 6].Value?.ToString().Trim();
                            var maLop = worksheet.Cells[row, 7].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(maSv) || string.IsNullOrEmpty(maLop))
                            {
                                soDongLoi++;
                                loiChiTiet.Add($"Dòng {row}: Thiếu Mã SV hoặc Mã Lớp.");
                                continue;
                            }

                            if (_context.SinhViens.Any(s => s.MaSv == maSv))
                            {
                                soDongLoi++;
                                loiChiTiet.Add($"Dòng {row}: Mã SV {maSv} đã tồn tại.");
                                continue;
                            }

                            if (!_context.Lops.Any(l => l.MaLop == maLop))
                            {
                                soDongLoi++;
                                loiChiTiet.Add($"Dòng {row}: Mã Lớp {maLop} không tồn tại.");
                                continue;
                            }

                            DateOnly ngaySinh = DateOnly.FromDateTime(DateTime.Now);
                            if (ngaySinhRaw != null && ngaySinhRaw is DateTime)
                            {
                                ngaySinh = DateOnly.FromDateTime((DateTime)ngaySinhRaw);
                            }

                            var sv = new SinhVien
                            {
                                MaSv = maSv,
                                HoTen = hoTen,
                                NgaySinh = ngaySinh,
                                GioiTinh = gioiTinh,
                                DiaChi = diaChi,
                                Email = email,
                                MaLop = maLop,
                                TrangThaiHocTap = "Đang học",
                                DiemTrungBinh = 0
                            };

                            _context.Add(sv);
                            soDongThanhCong++;
                        }
                        catch (Exception ex)
                        {
                            soDongLoi++;
                            loiChiTiet.Add($"Dòng {row}: Lỗi định dạng ({ex.Message})");
                        }
                    }

                    if (soDongThanhCong > 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
            }

            ViewBag.Success = $"Đã import thành công {soDongThanhCong} sinh viên.";
            if (soDongLoi > 0)
            {
                ViewBag.Error = $"Có {soDongLoi} dòng bị lỗi. Chi tiết:";
                ViewBag.LoiChiTiet = loiChiTiet;
            }

            return View();
        }

        // -----------------------------------------------------------
        // EXPORT EXCEL
        // -----------------------------------------------------------
        public async Task<IActionResult> Export()
        {
            var danhSachSV = await _context.SinhViens.Include(s => s.MaLopNavigation).ToListAsync();


            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachSinhVien");

                // --- HEADER ---
                worksheet.Cells[1, 1].Value = "Mã SV";
                worksheet.Cells[1, 2].Value = "Họ Tên";
                worksheet.Cells[1, 3].Value = "Ngày Sinh";
                worksheet.Cells[1, 4].Value = "Giới Tính";
                worksheet.Cells[1, 5].Value = "Lớp";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Trạng Thái";
                worksheet.Cells[1, 8].Value = "Điểm TB Tích Lũy";

                using (var range = worksheet.Cells["A1:H1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                foreach (var sv in danhSachSV)
                {
                    worksheet.Cells[row, 1].Value = sv.MaSv;
                    worksheet.Cells[row, 2].Value = sv.HoTen;

                    worksheet.Cells[row, 3].Value = sv.NgaySinh.ToString("dd/MM/yyyy");

                    worksheet.Cells[row, 4].Value = sv.GioiTinh;
                    worksheet.Cells[row, 5].Value = sv.MaLop;
                    worksheet.Cells[row, 6].Value = sv.Email;
                    worksheet.Cells[row, 7].Value = sv.TrangThaiHocTap;
                    worksheet.Cells[row, 8].Value = sv.DiemTrungBinh;

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                await package.SaveAsAsync(stream);
                stream.Position = 0;

                string excelName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}