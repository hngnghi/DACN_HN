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
    public class HocKiesController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public HocKiesController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: HocKies
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocKies.ToListAsync());
        }

        // GET: HocKies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKies
                .FirstOrDefaultAsync(m => m.MaHk == id);
            if (hocKy == null)
            {
                return NotFound();
            }

            return View(hocKy);
        }

        // GET: HocKies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HocKies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHk,TenHk,NamHoc")] HocKy hocKy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocKy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hocKy);
        }

        // GET: HocKies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKies.FindAsync(id);
            if (hocKy == null)
            {
                return NotFound();
            }
            return View(hocKy);
        }

        // POST: HocKies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHk,TenHk,NamHoc")] HocKy hocKy)
        {
            if (id != hocKy.MaHk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocKy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocKyExists(hocKy.MaHk))
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
            return View(hocKy);
        }

        // GET: HocKies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKies
                .FirstOrDefaultAsync(m => m.MaHk == id);
            if (hocKy == null)
            {
                return NotFound();
            }

            return View(hocKy);
        }

        // POST: HocKies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocKy = await _context.HocKies.FindAsync(id);
            if (hocKy != null)
            {
                _context.HocKies.Remove(hocKy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocKyExists(string id)
        {
            return _context.HocKies.Any(e => e.MaHk == id);
        }
    }
}
