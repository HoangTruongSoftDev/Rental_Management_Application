using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RentalManagementFinalProject.Models;

namespace RentalManagementFinalProject.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly RentalManagementDbContext _context;

        public AppointmentsController(RentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var rentalManagementDbContext = _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant);
            return View(await rentalManagementDbContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchType, string searchString, DateTime? searchDateFrom = null, DateTime? searchDateTo = null)
        {
            if (string.IsNullOrEmpty(searchString) && searchType == "AppointmentReason" )
            {
                ViewData["ErrorMessage"] = "Missing Search Text";
                var rentalManagementDbContext = _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant);
                return View(await rentalManagementDbContext.ToListAsync());
            }
            switch (searchType)
            {
                case "AppointmentReason":
                    return View(await _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant)
                        .Where(a => a.AppointmentReason.ToLower().Trim().Contains(searchString.ToLower().Trim()))
                        .ToListAsync());
                case "MeetingDateTime":
                    return View(await _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant)
                        .Where(m => m.MeetingDateTime >= searchDateFrom && m.MeetingDateTime <= searchDateTo).ToListAsync());          
                case "All":
                    ViewData["ErrorMessage"] = null;
                    return View(await _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant).ToListAsync());
                default:
                    return View(await _context.Appointments.Include(a => a.Apartment).Include(a => a.PropertyManager).Include(a => a.Tenant).ToListAsync());
            }
        }
        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment)
                .Include(a => a.PropertyManager)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentNumber", "ApartmentNumber");
            ViewData["PropertyManagerId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["TenantId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["listOfApartmentNumber"] = new SelectList(_context.Apartments, "ApartmentNumber", "ApartmentNumber");
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                if (sessionUser.UserTypeId == 1)
                {
                    ViewData["listOfPropertyManager"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 2), "UserId", "FirstName");
                    ViewData["listOfTenant"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 3), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 2)
                {
                    ViewData["listOfTenant"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 3)), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 3)
                {
                    ViewData["listOfPropertyManager"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 2)), "UserId", "FirstName");
                }
                return View();
            }
            
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PropertyManagerId,TenantId,ApartmentId,MeetingDateTime,AppointmentReason")] Appointment appointment)
        {
            if (string.IsNullOrEmpty(appointment.AppointmentReason))
            {
                ViewData["ErrorMessage"] = "Missing Purpose of Appointment";
                return View();
            }
            int appointmentId = _context.Appointments.Max(m => m.AppointmentId) + 1;
            appointment.AppointmentId = appointmentId;
            appointment.Tenant = _context.Users.SingleOrDefault(currentUser => currentUser.UserId == appointment.TenantId);
            appointment.PropertyManager = _context.Users.SingleOrDefault(currentUser => currentUser.UserId == appointment.PropertyManagerId);
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentNumber", "ApartmentNumber", appointment.ApartmentId);
            ViewData["PropertyManagerId"] = new SelectList(_context.Users, "UserId", "UserId", appointment.PropertyManagerId);
            ViewData["TenantId"] = new SelectList(_context.Users, "UserId", "UserId", appointment.TenantId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PropertyManagerId,TenantId,ApartmentId,MeetingDateTime,AppointmentReason")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentNumber", "ApartmentNumber", appointment.ApartmentId);
            ViewData["PropertyManagerId"] = new SelectList(_context.Users, "UserId", "UserId", appointment.PropertyManagerId);
            ViewData["TenantId"] = new SelectList(_context.Users, "UserId", "UserId", appointment.TenantId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment)
                .Include(a => a.PropertyManager)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
