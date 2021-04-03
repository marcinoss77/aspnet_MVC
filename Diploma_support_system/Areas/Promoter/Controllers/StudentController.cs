using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Diploma_support_system.Data;
using Diploma_support_system.Models;
using Diploma_support_system.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma_support_system.Areas.Promoter.Controllers
{
    [Authorize(Roles="Promoter")]
    [Area("Promoter")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;


        [BindProperty]
        public StudentDiplomaViewModel StudentDiplomaVM { get; set; }
        public IndexViewModel IndexVM { get; set; }

        public StudentController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment,UserManager<IdentityUser>userManager)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
         

            StudentDiplomaVM = new StudentDiplomaViewModel()
            {
                Promoter = _db.Promoter,
                Student = new Models.Student()
            };
        }
        public async Task<IActionResult> Index()
        {
            var students = await _db.Student.Include(m => m.Promoter).Include(m => m.Group).ToListAsync();
            return View(students);
        }
        //public async Task<IActionResult> StudentIndex()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var name = user.Name;
        //    var surname = user.Surname;
        //    // var claimIndentity = (ApplicationUser)User;
        //    //var name = claimIndentity.Name.ToString();
        //    //  var surname = claimIndentity.Surname;
        //    //            var claims = claimIndentity.FindFirst(ClaimTypes.Surname).ToString();
        //    //            var claimv2 = claimIndentity.FindFirst(ClaimTypes.Name).ToString();

        //    //  var user = await _db.ApplicationUsers.//FirstOrDefaultAsync(m => m.Id == id);

        //    //IndexViewModel IndexVM = new IndexViewModel()
        //    //{
        //    //    Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group).ToListAsync(),
        //    //    //Promoter = (IEnumerable<Models.Promoter>)applicationUser.Name.ToList()
        //    //   // Promoter = await _db.Promoter.Where(m => m.Name.Equals(claims)).ToListAsync()
        //    //};
        //    //return View(IndexVM);



        //    //string promoterName = applicationUser.Name;


            
        //    //return View(promoterName);
        //    return View(promoter);
        //}
        public IActionResult Create()
        {
            return View(StudentDiplomaVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            StudentDiplomaVM.Student.GroupId = Convert.ToInt32(Request.Form["GroupId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(StudentDiplomaVM);
            }

            _db.Student.Add(StudentDiplomaVM.Student);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var studentFromDb = await _db.Student.FindAsync(StudentDiplomaVM.Student.Id);

            if (files.Count > 0)
            {
                //file been uploaded
                var uploads = Path.Combine(webRootPath, "diplomas");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, StudentDiplomaVM.Student.Name + "_" + StudentDiplomaVM.Student.Surname + extension),
                    FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                studentFromDb.Diploma = StudentDiplomaVM.Student.Name+"_"+ StudentDiplomaVM.Student.Surname + extension;
            }
            else
            {
                //no file uploadede
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult DownloadFile(string filePath, string name, string surname)
        {
            string extension = Path.GetExtension(filePath);
            string webRootPath = _hostingEnvironment.WebRootPath;

            var finalPath = webRootPath + "\\diplomas\\"+ filePath;

            byte[] fileBytes = System.IO.File.ReadAllBytes(finalPath);

            string fileName = $"{name}_{surname}{extension}";

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentDiplomaVM.Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group)
                .SingleOrDefaultAsync(m => m.Id == id);
            StudentDiplomaVM.Group = await _db.Group.Where(s => s.PromoterId == StudentDiplomaVM.Student.PromoterId)
                .ToListAsync();
            
   

            if (StudentDiplomaVM.Group == null)
            {
                return NotFound();
            }

            return View(StudentDiplomaVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            StudentDiplomaVM.Student.GroupId = Convert.ToInt32(Request.Form["GroupId"].ToString());

            if (!ModelState.IsValid)
            {
                StudentDiplomaVM.Group = await _db.Group.Where(s => s.PromoterId == StudentDiplomaVM.Student.PromoterId).ToListAsync();
                return View(StudentDiplomaVM);
            }

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var studentFromDb = await _db.Student.FindAsync(StudentDiplomaVM.Student.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "diplomas");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var filePath = Path.Combine(webRootPath, studentFromDb.Diploma.TrimStart('\\'));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, StudentDiplomaVM.Student.Name + "_" + StudentDiplomaVM.Student.Surname + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                studentFromDb.Diploma = StudentDiplomaVM.Student.Name + "_" + StudentDiplomaVM.Student.Surname + extension_new;
            }

            studentFromDb.Name = StudentDiplomaVM.Student.Name;
            studentFromDb.Surname = StudentDiplomaVM.Student.Surname;
            studentFromDb.Description = StudentDiplomaVM.Student.Description;
            studentFromDb.DiplomaName = StudentDiplomaVM.Student.DiplomaName;
            studentFromDb.PromoterId = StudentDiplomaVM.Student.PromoterId;
            studentFromDb.GroupId = StudentDiplomaVM.Student.GroupId;
          

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentDiplomaVM.Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group)
                .SingleOrDefaultAsync(m => m.Id == id);
            StudentDiplomaVM.Group = await _db.Group.Where(s => s.PromoterId == StudentDiplomaVM.Student.PromoterId)
                .ToListAsync();

            if (StudentDiplomaVM.Student == null)
            {
                return NotFound();
            }

            return View(StudentDiplomaVM);
        }
        //POST - Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {
            return RedirectToAction(nameof(Edit), new { id });
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentDiplomaVM.Student = await _db.Student.Include(m => m.Promoter).Include(m => m.Group)
                .SingleOrDefaultAsync(m => m.Id == id);


            if (StudentDiplomaVM.Student == null)
            {
                return NotFound();
            }

            return View(StudentDiplomaVM);
        }
        //Post -delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Models.Student student = await _db.Student.FindAsync(id);



            if (student != null)
            {

                //Delete the original file
                var imagePath = Path.Combine(webRootPath+"\\diplomas\\", student.Diploma.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.Student.Remove(student);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

    }
}
