using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiCademy.Repository;
using AiCademy.Domain.Models;
using AiCademy.Service.Interface;

namespace AiCademy.Web.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;

        public LessonsController(ApplicationDbContext context, ILessonService lessonService, ICourseService courseService)
        {
            _context = context;
            _lessonService = lessonService;
            _courseService = courseService;
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lessons.Include(l => l.Course);
            return View(_lessonService.GetLessons());
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _lessonService.GetLessonById(id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Lessons/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_courseService.GetCourses(), "Id", "Title");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,CourseId")] Lesson lesson, IFormFile LessonFile)
        {
            if (ModelState.IsValid)
            {
                if (LessonFile != null && LessonFile.Length > 0)
                {
                    var originalFileName = Path.GetFileName(LessonFile.FileName);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(LessonFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await LessonFile.CopyToAsync(stream);
                    }

                    lesson.PresentationUrl = $"/uploads/{fileName}";
                    lesson.Title += $" ({originalFileName})";
                    
                    // OLD:
                    //lesson.FilePath = $"/uploads/{fileName}";
                    //lesson.Name += $" ({originalFileName})";
                }

                //lesson.Id = Guid.NewGuid();
                //_context.Add(lesson);
                _lessonService.CreateNewLesson(lesson);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_courseService.GetCourses(), "Id", "Title", lesson.CourseId);
            return View(lesson);
        }



        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _lessonService.GetLessonById(id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_courseService.GetCourses(), "Id", "Id", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,FilePath,CourseId,Id")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _lessonService.UpdateLesson(lesson);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_courseService.GetCourses(), "Id", "Id", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _lessonService.GetLessonById(id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            _lessonService.DeleteLesson(id);
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(Guid id)
        {
            return _lessonService.GetLessonById(id) != null;
        }
    }
}
