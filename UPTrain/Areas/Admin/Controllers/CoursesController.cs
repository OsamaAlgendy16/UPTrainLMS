using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPTrain.Data;
using UPTrain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using UPTrain.IRepositories;
using System.Linq.Expressions;

namespace UPTrain.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepo;
        private readonly UserManager<User> _userManager;

        public CoursesController(ICourseRepository courseRepo, UserManager<User> userManager)
        {
            _courseRepo = courseRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepo.GetAllAsync();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Courses course)
        {

          
            course.CreatedDate = DateTime.Now;
            await _courseRepo.AddAsync(course);

            TempData["SuccessMessage"] = "Course created successfully!";
            await _courseRepo.CommitAsync();

            

            return RedirectToAction(nameof(Index));

          
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var course = await _courseRepo.GetOneAsync(c => c.CourseId == id);

            if (course is not null)
            {
                var users = await _userManager.Users.ToListAsync();
                ViewBag.Users = new SelectList(users, "Id", "UserName", course.CreatedBy);
                return View(course);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Courses course)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userManager.Users.ToListAsync();
                ViewBag.Users = new SelectList(users, "Id", "UserName", course.CreatedBy);
                return View(course);
            }


            course.UpdatedDate = DateTime.Now;
            await _courseRepo.Update(course);

            TempData["SuccessMessage"] = "Course updated successfully!";
            await _courseRepo.CommitAsync();

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var course = await _courseRepo.GetOneAsync(c => c.CourseId == id);

            if (course is not null)
            {
               await _courseRepo.Delete(course);
                await _courseRepo.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}