// Developed by Josue Lopez Lozano
// Last Updated April 1st, 2024
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Data;
using TaskMasterPro.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TaskMasterPro.Controllers;
using Microsoft.Extensions.Logging;
using Amazon.Runtime.Internal.Util;

namespace TaskMasterPro.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve the logged-in user's ID
            var projects = await _context.Projects
                                .Where(p => p.UserId == userId) // Filter projects by the UserId
                                .ToListAsync();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var project = await _context.Projects
                .Where(p => p.UserId == userId && p.ProjectId == id) // Ensure the project belongs to the logged-in user
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Name,Description")] Project project)
        {
            // Remove ModelState entries for navigation properties that aren't part of the form submission.
            ModelState.Remove("User");
            ModelState.Remove("Tasks");

            // Retrieve the logged-in user's ID from the session.
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                // If the user ID exists in the session, assign it to the project.
                project.UserId = userId.Value;
            }
            else
            {
                // If there's no user ID in the session, add a model error. 
                //  might want to redirect to a login page or show an error message.
                ModelState.AddModelError("", "Your session has expired. Please log in again.");
            }

            // Check if the ModelState is valid after adjustments.
            if (ModelState.IsValid)
            {
                // If valid, may add the project to the database context and save changes.
                _context.Add(project);
                await _context.SaveChangesAsync();

                // Redirect to the index action method (the list of projects), showing the newly created project.
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the Create view with the current project model 
            // (including any errors from ModelState to be displayed).
            return View(project);
        }



        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name,Description")] Project project)
        {
            ModelState.Remove("User"); // Assuming there's a User navigation property causing the issue
            ModelState.Remove("Tasks"); // Assuming there's a Tasks collection causing the issue

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError("", "Session expired. Please log in again.");
                return View(project);
            }

            // Explicitly set the UserId to ensure it matches the current session's UserId
            project.UserId = userId.Value;

            if (id != project.ProjectId || project.UserId != userId)
            {
                return NotFound("Unauthorized access or incorrect project.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If there's a problem with the model state, return to the edit view with the current project model.
            return View(project);
        }



        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var project = await _context.Projects
                .Where(p => p.UserId == userId && p.ProjectId == id) // Ensure the project belongs to the logged-in user
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}

