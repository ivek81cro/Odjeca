using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models;
using Odjeca.Utility;

namespace Odjeca.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BrandsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Index()
        {
            return View(await _db.Brands.ToListAsync());
        }

        // GET: Admin/Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brands = await _db.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brands == null)
            {
                return NotFound();
            }

            return View(brands);
        }

        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brands brands)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    //saving picture to database as byte of streams
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        brands.Image = p1;
                    }
                    _db.Brands.Add(brands);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(brands);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brands = await _db.Brands.FindAsync(id);
            if (brands == null)
            {
                return NotFound();
            }
            return View(brands);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Brands brands)
        {
            if (id != brands.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(brands);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandsExists(brands.Id))
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
            return View(brands);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brands = await _db.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brands == null)
            {
                return NotFound();
            }

            return View(brands);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brands = await _db.Brands.FindAsync(id);
            _db.Brands.Remove(brands);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandsExists(int id)
        {
            return _db.Brands.Any(e => e.Id == id);
        }
    }
}
