using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models.ViewModels;

namespace Odjeca.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public StoreItemViewModel StoreItemVM { get; set; }

        public StoreItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnviroment)
        {
            _db = db;
            _hostingEnviroment = hostingEnviroment;
            StoreItemVM = new StoreItemViewModel()
            {
                Category = _db.Category,
                StoreItem = new Models.StoreItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            var storeItems = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(storeItems);
        }

        //GET-Create
        public IActionResult Create()
        {
            return View(StoreItemVM);
        }
    }
}