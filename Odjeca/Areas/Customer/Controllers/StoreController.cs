using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models;
using Odjeca.Models.ViewModels;
using Odjeca.Utility;

namespace Odjeca.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StoreController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            StoreViewModel StoreVM = new StoreViewModel()
            {
                StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                SubCategory = await _db.SubCategory.ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            return View(StoreVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var storeItemfromDb = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                StoreItem = storeItemfromDb,
                StoreItemId = storeItemfromDb.Id
            };

            return View(cartObj);
        }

        public async Task<IActionResult> DetailsModal(int id)
        {
            var storeItemfromDb = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                StoreItem = storeItemfromDb,
                StoreItemId = storeItemfromDb.Id
            };

            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == cartObject.ApplicationUserId
                                            && c.StoreItemId == cartObject.StoreItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(cartObject);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cartObject.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == cartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var storeItemFromDb = await _db.StoreItem.Include(m => m.Category)
                    .Include(m => m.SubCategory).Where(m => m.Id == cartObject.StoreItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    StoreItem = storeItemFromDb,
                    StoreItemId = storeItemFromDb.Id
                };

                return View(cartObj);
            }
        }
    }
}
