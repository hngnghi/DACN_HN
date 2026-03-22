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
    [Authentication(Roles = "Admin")]
    public class KhoasController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public KhoasController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: Khoas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Khoas.ToListAsync());
        }

        // GET: Khoas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas
                .FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
            {
                return NotFound();
            }

            return View(khoa);
        }

        // GET: Khoas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhoa,TenKhoa,TruongKhoa")] Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        // GET: Khoas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }
            return View(khoa);
        }

        // POST: Khoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoa,TenKhoa,TruongKhoa")] Khoa khoa)
        {
            if (id != khoa.MaKhoa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaExists(khoa.MaKhoa))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        // GET: Khoas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas
                .FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
            {
                return NotFound();
            }

            return View(khoa);
        }

        // POST: Khoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Thử xóa
                _context.Khoas.Remove(khoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Xóa được thì quay về danh sách
            }
            catch (DbUpdateException) { 

                // 1. Tạo thông báo lỗi để hiển thị ra màn hình
                ViewBag.Error = "Không thể xóa Khoa này vì đang có Lớp hoặc Giảng viên thuộc về nó. Vui lòng xóa dữ liệu liên quan trước!";

                // 2. Trả về lại trang Xóa (Delete) để người dùng đọc thông báo
                return View("Delete", khoa);
            }
        }

        private bool KhoaExists(string id)
        {
            return _context.Khoas.Any(e => e.MaKhoa == id);
        }
    }
}
