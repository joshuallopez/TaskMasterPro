// Developed by Josue Lopez Lozano
// Last Updated April 1st, 2024

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Data;
using TaskMasterPro.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Task = TaskMasterPro.Models.Task;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskMasterPro.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve the logged-in user's ID
            // Include the Project navigation property and filter by the logged-in user's ID
            var tasks = await _context.Tasks
                            .Include(t => t.Project)
                            .Where(t => t.UserId == userId) // Filter tasks by UserId
                            .ToListAsync();
            return View(tasks);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.TaskId == id && m.UserId == userId); // Ensure the task belongs to the logged-in user

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            // Provide a list of projects to select from when creating a task, filtered by the logged-in user's ID
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.Projects = new SelectList(_context.Projects.Where(p => p.UserId == userId), "ProjectId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Title,Description,DueDate,IsComplete,ProjectId")] Task task)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError("", "Session expired. Please log in again.");
            }
            else
            {
                task.UserId = userId.Value; // Ensuring the task is associated with the logged-in user
            }

            ModelState.Remove("User");
            ModelState.Remove("Project");

            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Ensuring the dropdown list for projects is filtered by the logged-in user
            ViewBag.Projects = new SelectList(_context.Projects.Where(p => p.UserId == userId), "ProjectId", "Name", task.ProjectId);
            return View(task);
        }


        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != userId)
            {
                return NotFound();
            }

            ViewBag.Projects = new SelectList(_context.Projects.Where(p => p.UserId == userId), "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,Title,Description,DueDate,IsComplete,ProjectId")] Task task)
        {
            ModelState.Remove("Project");
            ModelState.Remove("User");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError("", "Session expired. Please log in again.");
                return View(task);
            }

            // Explicitly set the UserId to ensure it matches the current session's UserId
            task.UserId = userId.Value;

            if (id != task.TaskId || task.UserId != userId)
            {
                return NotFound("Unauthorized access or incorrect task.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate Projects for dropdown
            ViewBag.Projects = new SelectList(_context.Projects.Where(p => p.UserId == userId), "ProjectId", "Name", task.ProjectId);
            return View(task);
        }




        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.TaskId == id && m.UserId == userId); // Ensuring the task belongs to the logged-in user

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null && task.UserId == HttpContext.Session.GetInt32("UserId"))
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
