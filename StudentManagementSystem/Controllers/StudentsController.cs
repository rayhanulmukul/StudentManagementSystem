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
        private const string SearchTermSessionKey = "Students.SearchTerm";
        private const string PageNumberSessionKey = "Students.PageNumber";

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
        {
            try
            {
                var hasQueryState = Request.Query.ContainsKey("searchTerm") || Request.Query.ContainsKey("pageNumber");

                if (hasQueryState)
                {
                    var normalizedSearchTerm = NormalizeSearchTerm(searchTerm);
                    var requestedPage = Math.Max(pageNumber ?? 1, 1);
                    int requestedCount = await _studentService.GetTotalStudentCountAsync(normalizedSearchTerm);
                    int requestedTotalPages = Math.Max(1, (int)Math.Ceiling(requestedCount / (double)PageSize));
                    int correctedPage = Math.Min(requestedPage, requestedTotalPages);

                    SetListState(normalizedSearchTerm, correctedPage);
                    return RedirectToAction(nameof(Index));
                }

                var activeSearchTerm = HttpContext.Session.GetString(SearchTermSessionKey);
                int requestedSessionPage = HttpContext.Session.GetInt32(PageNumberSessionKey) ?? 1;
                int totalCount = await _studentService.GetTotalStudentCountAsync(activeSearchTerm);
                int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
                int currentPage = Math.Min(Math.Max(requestedSessionPage, 1), totalPages);

                SetListState(activeSearchTerm, currentPage);

                var students = await _studentService.GetStudentsPaginatedAsync(currentPage, PageSize, activeSearchTerm);

                ViewData["CurrentPage"] = currentPage;
                ViewData["TotalPages"] = totalPages;
                ViewData["SearchTerm"] = activeSearchTerm;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string? searchTerm)
        {
            SetListState(NormalizeSearchTerm(searchTerm), 1);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePage(int pageNumber)
        {
            var activeSearchTerm = HttpContext.Session.GetString(SearchTermSessionKey);
            SetListState(activeSearchTerm, Math.Max(pageNumber, 1));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSearch()
        {
            ClearListState();
            return RedirectToAction(nameof(Index));
        }

        private void SetListState(string? searchTerm, int pageNumber)
        {
            var normalizedSearchTerm = NormalizeSearchTerm(searchTerm);

            if (string.IsNullOrEmpty(normalizedSearchTerm))
            {
                HttpContext.Session.Remove(SearchTermSessionKey);
            }
            else
            {
                HttpContext.Session.SetString(SearchTermSessionKey, normalizedSearchTerm);
            }

            HttpContext.Session.SetInt32(PageNumberSessionKey, Math.Max(pageNumber, 1));
        }

        private void ClearListState()
        {
            HttpContext.Session.Remove(SearchTermSessionKey);
            HttpContext.Session.Remove(PageNumberSessionKey);
        }

        private static string? NormalizeSearchTerm(string? searchTerm)
        {
            return string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        }
    }
}
