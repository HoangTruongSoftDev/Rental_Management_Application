using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RentalManagementFinalProject.Models;
using System.Security.Claims;
using Newtonsoft.Json;

namespace RentalManagementFinalProject.Controllers
{
    public class AccessController : Controller
    {
        private readonly RentalManagementDbContext _context;
        public AccessController(RentalManagementDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                switch (HttpContext.Session.GetString("UserType"))
                {
                    case "Property Owner"://Property Admin
                        return RedirectToAction("Index", "Users");
                    case "Property Manager"://Property Manager
                        return RedirectToAction("Index", "Buildings");
                    case "Tenant"://Tenant
                        return RedirectToAction("Index", "Apartments");
                    default:
                        return View();
                }
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Undefined");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("ErrorMessage", "Missing Username");
                return View();
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("ErrorMessage", "Missing Password");
                return View();
            }
            var existingUser = _context.Users.SingleOrDefault(currentUser => currentUser.UserName == user.UserName);
            if (existingUser == null)
            {
                ModelState.AddModelError("ErrorMessage", "Invalid User");
                return View();
            }
            if (existingUser.UserTypeId != user.UserTypeId)
            {
                ModelState.AddModelError("ErrorMessage", "Your User Type is incorrect");
                return View();
            }
            if (existingUser.Password != user.Password)
            {
                ModelState.AddModelError("ErrorMessage", "Invalid Password");
                return View();
            }
            if (existingUser.StatusId == 2)
            {
                ModelState.AddModelError("ErrorMessage", "User account is no longer available");
                return View();
            }
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, Convert.ToString(user.UserType))
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            switch (existingUser.UserTypeId)
            {
                case 1://Property Admin
                    HttpContext.Session.SetString("UserType", "Property Owner");
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(existingUser));
                    return RedirectToAction("Index", "Users");
                case 2://Property Manager
                    HttpContext.Session.SetString("UserType", "Property Manager");
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(existingUser));
                    return RedirectToAction("Index", "Apartments");
                case 3://Tenant
                    HttpContext.Session.SetString("UserType", "Tenant");
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(existingUser));
                    return RedirectToAction("Index", "Appointments");
                default:
                    ModelState.AddModelError("ErrorMessage", "Please select your user type");
                    return View();
            }
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User user)
        {

            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("ErrorMessage", "Missing Username");
                return View();
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                ModelState.AddModelError("ErrorMessage", "Missing FirstName");
                return View();
            }
            if (string.IsNullOrEmpty(user.LastName))
            {
                ModelState.AddModelError("ErrorMessage", "Missing LastName");
                return View();
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("ErrorMessage", "Missing Password");
                return View();
            }
            var existingUser = _context.Users.SingleOrDefault(currentUser => currentUser.UserName == user.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("ErrorMessage", "This user already exist");
                return View();
            }
            int userId = _context.Users.Max(user => user.UserId) + 1;
            user.UserId = userId;
            user.StatusId = 1;
            user.UserTypeId = 3;
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}
