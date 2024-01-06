using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalManagementFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
namespace RentalManagementFinalProject.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly RentalManagementDbContext _context;
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetString("UserType", "Undefined");
            return RedirectToAction("Login", "Access");
        }
        public UsersController(RentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
                var rentalManagementDbContext = _context.Users.Include(u => u.Status).Include(u => u.UserType);
                return View(await rentalManagementDbContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchType, string searchString)
        {
            
                if (string.IsNullOrEmpty(searchString) && searchType != "All")
                {
                    ViewData["ErrorMessage"] = "Missing Search Text";
                    var rentalManagementDbContext = _context.Users.Include(u => u.Status).Include(u => u.UserType);
                    return View(await rentalManagementDbContext.ToListAsync());
                }
                switch (searchType)
                {
                    case "FirstName":
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType)
                            .Where(u => u.FirstName.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                    case "LastName":
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType)
                            .Where(u => u.LastName.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                    case "UserType":
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType)
                            .Where(u => u.UserType.Description.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                    case "Status":
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType)
                            .Where(u => u.Status.Description.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                    case "All":
                        ViewData["ErrorMessage"] = null;
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType).ToListAsync());
                    default:
                        return View(await _context.Users.Include(u => u.Status).Include(u => u.UserType).ToListAsync());
                }
        }
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Status)
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "UserTypeId", "UserTypeId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,Password,UserTypeId,StatusId,FirstName,LastName")] User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.UserName))
                {
                    ViewData["ErrorMessage"] = "Missing Username";
                    return View();
                }
                if (string.IsNullOrEmpty(user.FirstName))
                {
                    ViewData["ErrorMessage"] = "Missing First Name";
                    return View();
                }
                if (string.IsNullOrEmpty(user.LastName))
                {
                    ViewData["ErrorMessage"] = "Missing Last Name";
                    return View();
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    ViewData["ErrorMessage"] = "Missing Password";
                    return View();
                }
                var existingUser = _context.Users.SingleOrDefault(currentUser => currentUser.UserName == user.UserName);
                if (existingUser != null)
                {
                    ViewData["ErrorMessage"] = "This user already exist";
                    return View();
                }
                int userId = _context.Users.Max(user => user.UserId) + 1;
                user.UserId = userId;
                user.StatusId = 1;
                user.UserTypeId = 2;
                user.Status = _context.Statuses.SingleOrDefault(s => s.StatusId == user.StatusId);
                user.UserType = _context.UserTypes.SingleOrDefault(ut => ut.UserTypeId == user.UserTypeId);
                _context.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = e.Message; 
                Exception ex = e.InnerException;
                while (ex != null)
                {
                    ViewData["ErrorMessage"] += "\n===>>>" + ex.Message;
                    ex = ex.InnerException;
                }
                return View();
            }
           
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", user.StatusId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "UserTypeId", "UserTypeId", user.UserTypeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,Password,UserTypeId,StatusId,FirstName,LastName")] User user)
        {
           
            if (id != user.UserId)
            {
                return NotFound();
            }          
            try
            {
                if (string.IsNullOrEmpty(user.UserName))
                {
                    ViewData["ErrorMessage"] = "Missing Username";
                    return View();
                }
                if (string.IsNullOrEmpty(user.FirstName))
                {
                    ViewData["ErrorMessage"] = "Missing First Name";
                    return View();
                }
                if (string.IsNullOrEmpty(user.LastName))
                {
                    ViewData["ErrorMessage"] = "Missing Last Name";
                    return View();
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    ViewData["ErrorMessage"] = "Missing Password";
                    return View();
                }
                
                user.Status = _context.Statuses.SingleOrDefault(s => s.StatusId == user.StatusId);
                user.UserType = _context.UserTypes.SingleOrDefault(ut => ut.UserTypeId == user.UserTypeId);
                try
                {
                    _context.Update(user);
                }
                catch (Exception e)
                {
                    ViewData["ErrorMessage"] = e.Message;
                    Exception exception = e.InnerException;
                    while (exception != null)
                    {
                        ViewData["ErrorMessage"] += "\n==>>>" + exception.Message;
                        exception = exception.InnerException;
                    }
                    return View();
                }
                
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Status)
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.StatusId = 2;
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
