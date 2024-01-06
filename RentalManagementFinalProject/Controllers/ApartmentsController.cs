using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RentalManagementFinalProject.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalManagementFinalProject.Controllers
{
    //[Authorize]
    public class ApartmentsController : Controller
    {
        private readonly RentalManagementDbContext _context;

        public ApartmentsController(RentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            var rentalManagementDbContext = _context.Apartments.Include(a => a.Building).Include(a => a.Status);
            return View(await rentalManagementDbContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchType, string searchString, int searchMinPrice = Int32.MinValue, int searchMaxPrice = Int32.MaxValue, int searchNumberOfRoom = 0)
        {

            if (string.IsNullOrEmpty(searchString) && (searchType == "Description" || searchType == "BuildingName"))
            {
                ViewData["ErrorMessage"] = "Missing Search Text";
                var rentalManagementDbContext = _context.Apartments.Include(a => a.Building).Include(a => a.Status);
                return View(await rentalManagementDbContext.ToListAsync());
            }
            switch (searchType)
            {
                case "Description":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.Description.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                case "BuildingName":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.Building.BuildingName.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                case "Type":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.Type.ToLower().Trim() == searchString.ToLower().Trim()).ToListAsync());
                case "Status":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.Status.Description.ToLower().Trim() == searchString.ToLower().Trim()).ToListAsync());
                case "Price":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.Price >= searchMinPrice && a.Price <= searchMaxPrice).ToListAsync());
                case "NumberOfRoom":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.NumberOfRoom == searchNumberOfRoom).ToListAsync());
                case "NumberOfBathroom":
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status)
                        .Where(a => a.NumberOfBathroom == searchNumberOfRoom).ToListAsync());
                case "All":
                    ViewData["ErrorMessage"] = null;
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status).ToListAsync());
                default:
                    return View(await _context.Apartments.Include(a => a.Building).Include(a => a.Status).ToListAsync());
            }
        }
        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.ApartmentNumber == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // GET: Apartments/Create
        public IActionResult Create()
        {
            ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentNumber,BuildingId,StatusId,Description,Type,Price,NumberOfRoom,NumberOfBathroom")] Apartment apartment)
        {
            ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
            if (string.IsNullOrEmpty(apartment.ApartmentNumber.ToString()))
            {
                ViewData["ErrorMessage"] = "Missing Apartment Number";
                ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
                return View(apartment);
            }
            if (string.IsNullOrEmpty(apartment.Price.ToString()))
            {
                ViewData["ErrorMessage"] = "Missing Apartment Price";
                ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
                return View(apartment);
            }
            if (string.IsNullOrEmpty(apartment.Description))
            {
                ViewData["ErrorMessage"] = "Missing Apartment Description";
                ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
                return View(apartment);
            }
            var existingApartment = _context.Apartments.SingleOrDefault(a => a.ApartmentNumber == apartment.ApartmentNumber);
            if (existingApartment != null)
            {
                ViewData["ErrorMessage"] = "This apartment already exist";
                ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
                return View(apartment);
            }
            apartment.Status = _context.Statuses.SingleOrDefault(a => a.StatusId == apartment.StatusId);
            apartment.Building = _context.Buildings.SingleOrDefault(b => b.BuildingId == apartment.BuildingId);
            _context.Add(apartment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Apartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentNumber,BuildingId,StatusId,Description,Type,Price,NumberOfRoom,NumberOfBathroom")] Apartment apartment)
        {
            if (id != apartment.ApartmentNumber)
            {
                return NotFound();
            }
            try
            {
                ViewData["listOfBuildingName"] = new SelectList(_context.Buildings, "BuildingId", "BuildingName");
                if (string.IsNullOrEmpty(apartment.ApartmentNumber.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Apartment Number";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.BuildingId.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Building Name";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.StatusId.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Status";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.Description))
                {
                    ViewData["ErrorMessage"] = "Missing Description";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.Type))
                {
                    ViewData["ErrorMessage"] = "Missing Apartment Type";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.Price.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Apartment Price";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.NumberOfRoom.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Number Of Room";
                    return View();
                }
                if (string.IsNullOrEmpty(apartment.NumberOfBathroom.ToString()))
                {
                    ViewData["ErrorMessage"] = "Missing Number Of Bathroom";
                    return View();
                }
                apartment.Status = _context.Statuses.SingleOrDefault(a => a.StatusId == apartment.StatusId);
                apartment.Building = _context.Buildings.SingleOrDefault(b => b.BuildingId == apartment.BuildingId);
                try
                {
                    _context.Update(apartment);
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
                if (!ApartmentExists(apartment.ApartmentNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // GET: Apartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.ApartmentNumber == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentNumber == id);
        }
    }
}
