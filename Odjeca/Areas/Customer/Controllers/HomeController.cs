using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Odjeca.Data;
using Odjeca.Models;
using Odjeca.Models.ViewModels;
using Odjeca.Utility;

namespace Odjeca.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            StoreViewModel StoreVM = new StoreViewModel()
            {
                Discount = await _db.Discount.Where(c => c.IsActive == true).ToListAsync(),
                StoreItem = await _db.StoreItem.Where(s => s.Id != 0).ToListAsync()
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
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Brands()
        {
            StoreViewModel BrandsVM = new StoreViewModel()
            {
                Brands = await _db.Brands.Where(s => s.Id != 0).ToListAsync()
            };

            return View(BrandsVM);
        }

        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
