using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models;

namespace Odjeca.Areas.Admin.Controllers
{
    [Area(("Admin"))]
    public class DiscountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DiscountController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Discount.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        //Create-post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount discounts)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    //saving picture to database as byte of streams
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        discounts.Picture = p1;
                    }
                    _db.Discount.Add(discounts);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(discounts);
        }
        
        //GET-Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var discount = await _db.Discount.SingleOrDefaultAsync(m => m.Id == id);

            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        //POST-Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Discount discounts)
        {
            if (discounts.Id == 0)
            {
                return NotFound();
            }

            var discountFromDb = await _db.Discount.Where(c => c.Id == discounts.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    discountFromDb.Picture = p1;
                }
                discountFromDb.MinimumAmmount = discounts.MinimumAmmount;
                discountFromDb.Name = discounts.Name;
                discountFromDb.DiscountAmmount = discounts.DiscountAmmount;
                discountFromDb.DiscountType = discounts.DiscountType;
                discountFromDb.IsActive = discounts.IsActive;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discounts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _db.Discount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        //GET Delete Discount
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Discount.SingleOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discounts = await _db.Discount.SingleOrDefaultAsync(m => m.Id == id);
            _db.Discount.Remove(discounts);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}