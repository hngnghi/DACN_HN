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


namespace QUANLYSINHVIEN.Controllers
{
    [Authentication] // Yêu cầu đăng nhập cho toàn bộ Controller
    public class BangDiemsController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public BangDiemsController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // --- [HELPER METHODS] ---
        private void TinhDiemVaXepLoai(BangDiem bangDiem)
        {
            if (bangDiem.DiemQt.HasValue && bangDiem.DiemThi.HasValue)
            {
                double diemTB = (bangDiem.DiemQt.Value * 0.3) + (bangDiem.DiemThi.Value * 0.7);
                bangDiem.DiemTb = Math.Round(diemTB, 1);

                if (bangDiem.DiemTb >= 8.5) bangDiem.XepLoai = "Giỏi";
                else if (bangDiem.DiemTb >= 7.0) bangDiem.XepLoai = "Khá";
                else if (bangDiem.DiemTb >= 5.0) bangDiem.XepLoai = "Trung bình";
                else bangDiem.XepLoai = "Yếu";
            }
        }

        private async Task CapNhatGPASinhVien(string maSv)
        {
            var danhSachDiem = await _context.BangDiems
                                             .Include(b => b.MaMhNavigation)
                                             .Where(b => b.MaSv == maSv && b.DiemTb != null)
                                             .ToListAsync();

            if (danhSachDiem.Any())
            {
                double tongDiemTichLuy = 0;
                int tongTinChi = 0;

                foreach (var diem in danhSachDiem)
                {
                    if (diem.MaMhNavigation != null)
                    {
                        int tinChi = diem.MaMhNavigation.SoTinChi;
                        tongDiemTichLuy += (diem.DiemTb.Value * tinChi);
                        tongTinChi += tinChi;
                    }
                }

                if (tongTinChi > 0)
                {
                    double gpa = tongDiemTichLuy / tongTinChi;
                    var sinhVien = await _context.SinhViens.FindAsync(maSv);
                    if (sinhVien != null)
                    {
                        sinhVien.DiemTrungBinh = Math.Round(gpa, 2);
                        _context.Update(sinhVien);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        // ------------------------

        // GET: BangDiems
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var maSv = HttpContext.Session.GetString("MaSv");

            var query = _context.BangDiems
                .Include(b => b.MaHkNavigation)
                .Include(b => b.MaMhNavigation)
                .Include(b => b.MaSvNavigation)
                .AsQueryable();

            // [PHÂN QUYỀN DỮ LIỆU] Sinh viên chỉ xem được điểm của mình
            if (role == "SinhVien")
            {
                if (string.IsNullOrEmpty(maSv)) return RedirectToAction("Login", "TaiKhoans");
                query = query.Where(b => b.MaSv == maSv);
            }

            // Sắp xếp học kỳ mới nhất lên đầu
            query = query.OrderByDescending(b => b.MaHk);

            return View(await query.ToListAsync());
        }

        // GET: BangDiems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bangDiem = await _context.BangDiems
                .Include(b => b.MaHkNavigation)
                .Include(b => b.MaMhNavigation)
                .Include(b => b.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaBd == id);

            if (bangDiem == null) return NotFound();

            // [BẢO MẬT] Chặn SV xem chi tiết điểm của người khác qua URL
            var role = HttpContext.Session.GetString("Role");
            var maSv = HttpContext.Session.GetString("MaSv");
            if (role == "SinhVien" && bangDiem.MaSv != maSv)
            {
                return RedirectToAction("AccessDenied", "TaiKhoans");
            }

            return View(bangDiem);
        }

        // -------------------------------------------------------------------------
        // CÁC HÀM THAY ĐỔI DỮ LIỆU -> CHỈ ADMIN/GIẢNG VIÊN MỚI ĐƯỢC VÀO
        // -------------------------------------------------------------------------

        // GET: BangDiems/Create
        [Authentication(Roles = "Admin,GiangVien")]
        public IActionResult Create()
        {
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk");
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv");
            return View();
        }

        // POST: BangDiems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication(Roles = "Admin,GiangVien")]
        public async Task<IActionResult> Create([Bind("MaBd,MaSv,MaMh,MaHk,DiemQt,DiemThi,DiemTb,XepLoai")] BangDiem bangDiem)
        {
            TinhDiemVaXepLoai(bangDiem);

            // Xóa lỗi validation cho trường tự động tính
            ModelState.Remove("DiemTb");
            ModelState.Remove("XepLoai");
            ModelState.Remove("MaHkNavigation");
            ModelState.Remove("MaMhNavigation");
            ModelState.Remove("MaSvNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(bangDiem);
                await _context.SaveChangesAsync();

                if (bangDiem.MaSv != null) await CapNhatGPASinhVien(bangDiem.MaSv);

                return RedirectToAction(nameof(Index));
            }

            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", bangDiem.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", bangDiem.MaMh);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", bangDiem.MaSv);
            return View(bangDiem);
        }

        // GET: BangDiems/Edit/5
        [Authentication(Roles = "Admin,GiangVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bangDiem = await _context.BangDiems.FindAsync(id);
            if (bangDiem == null) return NotFound();

            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", bangDiem.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", bangDiem.MaMh);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", bangDiem.MaSv);
            return View(bangDiem);
        }

        // POST: BangDiems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication(Roles = "Admin,GiangVien")]
        public async Task<IActionResult> Edit(int id, [Bind("MaBd,MaSv,MaMh,MaHk,DiemQt,DiemThi,DiemTb,XepLoai")] BangDiem bangDiem)
        {
            if (id != bangDiem.MaBd) return NotFound();

            TinhDiemVaXepLoai(bangDiem);

            // Xóa lỗi validation khi edit
            ModelState.Remove("DiemTb");
            ModelState.Remove("XepLoai");
            ModelState.Remove("MaHkNavigation");
            ModelState.Remove("MaMhNavigation");
            ModelState.Remove("MaSvNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bangDiem);
                    await _context.SaveChangesAsync();

                    if (bangDiem.MaSv != null) await CapNhatGPASinhVien(bangDiem.MaSv);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BangDiemExists(bangDiem.MaBd)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", bangDiem.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", bangDiem.MaMh);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", bangDiem.MaSv);
            return View(bangDiem);
        }

        // GET: BangDiems/Delete/5
        [Authentication(Roles = "Admin,GiangVien")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bangDiem = await _context.BangDiems
                .Include(b => b.MaHkNavigation)
                .Include(b => b.MaMhNavigation)
                .Include(b => b.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaBd == id);
            if (bangDiem == null) return NotFound();

            return View(bangDiem);
        }

        // POST: BangDiems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authentication(Roles = "Admin,GiangVien")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bangDiem = await _context.BangDiems.FindAsync(id);
            string maSvCanCapNhat = null;

            if (bangDiem != null)
            {
                maSvCanCapNhat = bangDiem.MaSv;
                _context.BangDiems.Remove(bangDiem);
            }

            await _context.SaveChangesAsync();

            if (maSvCanCapNhat != null) await CapNhatGPASinhVien(maSvCanCapNhat);

            return RedirectToAction(nameof(Index));
        }

        private bool BangDiemExists(int id)
        {
            return _context.BangDiems.Any(e => e.MaBd == id);
        }
    }
}