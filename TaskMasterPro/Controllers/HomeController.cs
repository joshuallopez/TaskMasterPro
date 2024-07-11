// Developed by Josue Lopez Lozano
// Last Updated April 1st, 2024
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Models;
using TaskMasterPro.Data;

namespace TaskMasterPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Check if user is already logged in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                // Directly redirect logged-in users to the Dashboard
                return RedirectToAction("Dashboard");
            }
            return View(); // Present the signup form for new users
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(User user, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View("Index", user);
                }

                if (user.Password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Passwords do not match.");
                    return View("Index", user);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Log the user in automatically after registration
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetInt32("UserId", user.UserId); // Store user's ID in session for later use

                return RedirectToAction("Dashboard");
            }
            return View("Index", user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetInt32("UserId", user.UserId); // Important: Store the UserId in the session

                return RedirectToAction("Dashboard");
            }

            ViewData["ErrorMessage"] = "Invalid login attempt. Please try again.";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
