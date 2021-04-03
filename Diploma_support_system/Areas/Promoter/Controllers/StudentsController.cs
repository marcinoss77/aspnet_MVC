using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma_support_system.Data;
using Diploma_support_system.Models;
using Diploma_support_system.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diploma_support_system.Areas.Promoter.Controllers
{
    [Area("Promoter")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _db;



        public StudentsController(ApplicationDbContext db)
        {
            _db = db;


        }
        public async Task<IActionResult> Index()
        {


            
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group).ToListAsync(),
               // Promoter = await _db.Promoter.Where(m => m.Name.Equals(name)).ToListAsync()
                
            };
            return View(IndexVM);
        }
    }
  
}
