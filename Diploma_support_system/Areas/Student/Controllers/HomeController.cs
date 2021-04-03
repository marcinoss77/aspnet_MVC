using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Diploma_support_system.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Diploma_support_system.Models;
using Diploma_support_system.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Diploma_support_system.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IWebHostEnvironment _env;

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group).ToListAsync(),
                Promoter = await _db.Promoter.ToListAsync()
            };
            return View(IndexVM);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetImage()
        {
            string pathForDefaultUserImage = _env.WebRootPath + "\\images\\default_user.png";
            var imageFileStream = System.IO.File.OpenRead(pathForDefaultUserImage);
            return File(imageFileStream, "image/png");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
