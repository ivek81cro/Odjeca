using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models;
using Odjeca.Models.ViewModels;
using Odjeca.Utility;

namespace Odjeca.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class StoreItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;

        [BindProperty]
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

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            StoreItemVM.StoreItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(StoreItemVM);
            }

            _db.StoreItem.Add(StoreItemVM.StoreItem);
            await _db.SaveChangesAsync();

            //Image saving section
            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var storeItemFromDB = await _db.StoreItem.FindAsync(StoreItemVM.StoreItem.Id);

            if (files.Count > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads,StoreItemVM.StoreItem.Id+extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                storeItemFromDB.Image = @"\images\" + StoreItemVM.StoreItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultClothesImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + StoreItemVM.StoreItem.Id + ".png");
                storeItemFromDB.Image = @"\images\" + StoreItemVM.StoreItem.Id + ".png";
            }
            storeItemFromDB.ArrivalDate = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET-Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            StoreItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == StoreItemVM.StoreItem.CategoryId).ToListAsync();

            if (StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }
            return View(StoreItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            StoreItemVM.StoreItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                StoreItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == StoreItemVM.StoreItem.CategoryId).ToListAsync();
                return View(StoreItemVM);
            }

            //Image saving section
            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var storeItemFromDB = await _db.StoreItem.FindAsync(StoreItemVM.StoreItem.Id);

            if (files.Count > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //delete original file
                var imagePath = Path.Combine(webRootPath, storeItemFromDB.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //upload new file
                using (var fileStrean = new FileStream(Path.Combine(uploads, StoreItemVM.StoreItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStrean);
                }
                storeItemFromDB.Image = @"\images\" + StoreItemVM.StoreItem.Id + extension_new;
            }

            storeItemFromDB.Name = StoreItemVM.StoreItem.Name;
            storeItemFromDB.Description = StoreItemVM.StoreItem.Description;
            storeItemFromDB.Price = StoreItemVM.StoreItem.Price;
            storeItemFromDB.Manufacturer = StoreItemVM.StoreItem.Manufacturer;
            storeItemFromDB.SubCategoryId = StoreItemVM.StoreItem.SubCategoryId;
            storeItemFromDB.CategoryId = StoreItemVM.StoreItem.CategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }

            return View(StoreItemVM);
        }

        //GET-Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }

            return View(StoreItemVM);
        }

        //POST-Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnviroment.WebRootPath;
            StoreItem storeItem = await _db.StoreItem.FindAsync(id);

            if (storeItem != null)
            {
                var imagePath = Path.Combine(webRootPath, storeItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.StoreItem.Remove(storeItem);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}