using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPTrain.Data;
using UPTrain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UPTrain.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                                        .Include(c => c.Lessons)
                                        .Include(c => c.Quizzes)
                                        .Include(c => c.Creator)
                                        .ToListAsync();
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
            if (ModelState.IsValid)
            {
                try
                {
                    course.CreatedDate = DateTime.Now;
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Course created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", course.CreatedBy);
            return View(course);
        }

     
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", course.CreatedBy);

            return View(course);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Courses course)
        {
            if (id != course.CourseId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await _context.Courses.FindAsync(id);
                    if (existingCourse == null) return NotFound();

                    existingCourse.Title = course.Title;
                    existingCourse.Description = course.Description;
                    existingCourse.Category = course.Category;
                    existingCourse.ImageUrl = course.ImageUrl;
                    existingCourse.CreatedBy = course.CreatedBy;
                    existingCourse.UpdatedDate = DateTime.Now;

                    _context.Courses.Update(existingCourse);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", course.CreatedBy);
            return View(course);
        }


        public IActionResult Delete([FromRoute] int id)
        {
            var Cinema = _context.Courses.Find(id);

            if (Cinema is not null)
            {
                _context.Remove(Cinema);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
