using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma_support_system.Data;
using Diploma_support_system.Models;
using Diploma_support_system.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma_support_system.Areas.Promoter.Controllers
{
    //[Authorize(Roles = SD.DeanUser)]
    //[Authorize(Roles = SD.PromoterUser)]
    [Area("Promoter")]
    public class PromoterController : Controller
    {

        private readonly ApplicationDbContext _db;
        public PromoterController(ApplicationDbContext db)
        {
            _db = db;
        }
        //get
        public async Task<IActionResult> Index()
        {
            return View(await _db.Promoter.ToListAsync());
        }
        //get-create
        public IActionResult Create()
        {
            return View();
        }
        //post-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Promoter promoter)
        {
            if (ModelState.IsValid)
            {
                _db.Promoter.Add(promoter);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(promoter);
        }
        //Get -edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var promoter = await _db.Promoter.FindAsync(id);
            if (promoter == null)
            {
                return NotFound();
            }

            return View(promoter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Models.Promoter promoter)
        {
            if (ModelState.IsValid)
            {
                _db.Update(promoter);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(promoter);
            }
        }
        //Get - delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var promoter = await _db.Promoter.FindAsync(id);
            if (promoter == null)
            {
                return NotFound();
            }

            return View(promoter);
        }
        //Post -delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Promoter.FindAsync(id);
            _db.Promoter.Remove(category);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Get Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var promoter = await _db.Promoter.FindAsync(id);
            if (promoter == null)
            {
                return NotFound();
            }

            return View(promoter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {

            return RedirectToAction(nameof(Edit), new { id });
        }
    }
}
