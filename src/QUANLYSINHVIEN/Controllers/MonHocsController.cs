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
    public class MonHocsController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public MonHocsController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: MonHocs
        public async Task<IActionResult> Index()
        {
            var wquanlysinhvienContext = _context.MonHocs.Include(m => m.MaGvNavigation);
            return View(await wquanlysinhvienContext.ToListAsync());
        }

        // GET: MonHocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs
                .Include(m => m.MaGvNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // GET: MonHocs/Create
        public IActionResult Create()
        {
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv");
            return View();
        }

        // POST: MonHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMh,TenMh,SoTinChi,MaGv")] MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", monHoc.MaGv);
            return View(monHoc);
        }

        // GET: MonHocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null)
            {
                return NotFound();
            }
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", monHoc.MaGv);
            return View(monHoc);
        }

        // POST: MonHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaMh,TenMh,SoTinChi,MaGv")] MonHoc monHoc)
        {
            if (id != monHoc.MaMh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonHocExists(monHoc.MaMh))
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
            ViewData["MaGv"] = new SelectList(_context.GiangViens, "MaGv", "MaGv", monHoc.MaGv);
            return View(monHoc);
        }

        // GET: MonHocs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs
                .Include(m => m.MaGvNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // POST: MonHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHocs.Remove(monHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonHocExists(string id)
        {
            return _context.MonHocs.Any(e => e.MaMh == id);
        }
    }
}
