using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLYSINHVIEN.Data;
using QUANLYSINHVIEN.Models;

namespace QUANLYSINHVIEN.Controllers
{
    public class LopHocPhansController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public LopHocPhansController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: LopHocPhans
        public async Task<IActionResult> Index()
        {
            var wquanlysinhvienContext = _context.LopHocPhans.Include(l => l.MaGvNavigation).Include(l => l.MaHkNavigation).Include(l => l.MaMhNavigation);
            return View(await wquanlysinhvienContext.ToListAsync());
        }

        // GET: LopHocPhans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaHkNavigation)
                .Include(l => l.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaLhp == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Create
        public IActionResult Create()
        {
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv");
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk");
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh");
            return View();
        }

        // POST: LopHocPhans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLhp,MaMh,MaGv,MaHk,SiSo")] LopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHocPhan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", lopHocPhan.MaGv);
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", lopHocPhan.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", lopHocPhan.MaMh);
            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", lopHocPhan.MaGv);
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", lopHocPhan.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", lopHocPhan.MaMh);
            return View(lopHocPhan);
        }

        // POST: LopHocPhans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaLhp,MaMh,MaGv,MaHk,SiSo")] LopHocPhan lopHocPhan)
        {
            if (id != lopHocPhan.MaLhp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopHocPhan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocPhanExists(lopHocPhan.MaLhp))
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
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", lopHocPhan.MaGv);
            ViewData["MaHk"] = new SelectList(_context.HocKies, "MaHk", "MaHk", lopHocPhan.MaHk);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "MaMh", lopHocPhan.MaMh);
            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaHkNavigation)
                .Include(l => l.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaLhp == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }

        // POST: LopHocPhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan != null)
            {
                _context.LopHocPhans.Remove(lopHocPhan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LopHocPhanExists(string id)
        {
            return _context.LopHocPhans.Any(e => e.MaLhp == id);
        }
    }
}
