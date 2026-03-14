using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using System.Diagnostics;

namespace StudentManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;
        private const int PageSize = 10;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1)
        {
            try
            {
                int totalCount = await _studentService.GetTotalStudentCountAsync(searchTerm);
                var students = await _studentService.GetStudentsPaginatedAsync(pageNumber, PageSize, searchTerm);

                ViewData["CurrentPage"] = pageNumber;
                ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)PageSize);
                ViewData["SearchTerm"] = searchTerm;
                ViewData["TotalCount"] = totalCount;

                return View(students.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students");
                TempData["Error"] = "An error occurred while loading students.";
                return View(new List<Student>());
            }
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid student ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var student = await _studentService.GetStudentByIdAsync(id.Value);
                if (student == null)
                {
                    TempData["Error"] = $"Student with ID {id.Value} was not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student details");
                TempData["Error"] = "Could not load student details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,DateOfBirth,Address,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.CreateStudentAsync(student);
                    TempData["Success"] = "Student created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating student");
                    ModelState.AddModelError("", "An error occurred while creating the student.");
                }
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid student ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var student = await _studentService.GetStudentByIdAsync(id.Value);
                if (student == null)
                {
                    TempData["Error"] = $"Student with ID {id.Value} was not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student for edit");
                TempData["Error"] = "Could not load student for edit.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,DateOfBirth,Address,Status")] Student student)
        {
            if (id != student.Id)
            {
                TempData["Error"] = "Student ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _studentService.UpdateStudentAsync(id, student);
                    if (result == null)
                    {
                        TempData["Error"] = $"Student with ID {id} was not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["Success"] = "Student updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating student");
                    ModelState.AddModelError("", "An error occurred while updating the student.");
                }
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid student ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var student = await _studentService.GetStudentByIdAsync(id.Value);
                if (student == null)
                {
                    TempData["Error"] = $"Student with ID {id.Value} was not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student for delete");
                TempData["Error"] = "Could not load student for delete.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _studentService.DeleteStudentAsync(id);
                if (success)
                {
                    TempData["Success"] = "Student deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Student not found.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student");
                TempData["Error"] = "An error occurred while deleting the student.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Students/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var students = await _studentService.SearchStudentsAsync(searchTerm);
                ViewData["SearchTerm"] = searchTerm;
                return View("Index", students.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching students");
                return RedirectToAction(nameof(Index));
            }
        }

        // API endpoint for instant search without page reload
        [HttpGet("api/students/search")]
        [ResponseCache(Duration = 0)]
        public async Task<IActionResult> SearchApi(string? searchTerm, int pageNumber = 1)
        {
            try
            {
                int totalCount = await _studentService.GetTotalStudentCountAsync(searchTerm);
                var students = await _studentService.GetStudentsPaginatedAsync(pageNumber, PageSize, searchTerm);

                return Json(new
                {
                    success = true,
                    data = students.Select(s => new
                    {
                        s.Id,
                        s.FirstName,
                        s.LastName,
                        s.FullName,
                        s.Email,
                        s.PhoneNumber,
                        dateOfBirth = s.DateOfBirth.ToString("MMM dd, yyyy"),
                        s.Status
                    }),
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                    currentPage = pageNumber,
                    pageSize = PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching students via API");
                return Json(new { success = false, message = "Search failed" });
            }
        }
    }
}
