using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalManagementFinalProject.Models;
namespace RentalManagementFinalProject.Controllers
{
    [Authorize]
    public class BuildingsController : Controller
    {
        private readonly RentalManagementDbContext _context;

        public BuildingsController(RentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Buildings.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchType, string searchString)
        {

            if (string.IsNullOrEmpty(searchString) && searchType != "All")
            {
                ViewData["ErrorMessage"] = "Missing Search Text";
                var rentalManagementDbContext = _context.Buildings;
                return View(await rentalManagementDbContext.ToListAsync());
            }
            switch (searchType)
            {
                case "BuildingId":
                    return View(await _context.Buildings.Where(b => b.BuildingId.ToString().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                case "BuildingName":
                    return View(await _context.Buildings.Where(b => b.BuildingName.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                case "All":
                    ViewData["ErrorMessage"] = null;
                    return View(await _context.Buildings.ToListAsync());
                default:
                    return View(await _context.Buildings.ToListAsync());
            }
        }
        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .FirstOrDefaultAsync(m => m.BuildingId == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildingId,BuildingName")] Building building)
        {
            var existingBuilding = _context.Buildings.SingleOrDefault(b => b.BuildingId == building.BuildingId);
            if (existingBuilding != null)
            {
                ViewData["ErrorMessage"] = "This Building Number already exists";
                return View(building);
            }
            existingBuilding = _context.Buildings.SingleOrDefault(b => b.BuildingName == building.BuildingName);
            if (existingBuilding != null)
            {
                ViewData["ErrorMessage"] = "This Building Name already exists";
                return View(building);
            }
            if (ModelState.IsValid)
            {
                _context.Add(building);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BuildingId,BuildingName")] Building building)
        {
            if (id != building.BuildingId)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(building.BuildingName))
            {
                ViewData["ErrorMessage"] = "Missing Building Name";
                return View();
            }
            var existingBuilding = _context.Buildings.SingleOrDefault(b => b.BuildingName == building.BuildingName && id != b.BuildingId);
            if (existingBuilding != null) 
            {
                ViewData["ErrorMessage"] = "This Building Name already exists";
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(building);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.BuildingId))
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
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .FirstOrDefaultAsync(m => m.BuildingId == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var building = await _context.Buildings.Include(b => b.Apartments).FirstOrDefaultAsync(b => b.BuildingId == id);
            if (building != null)
            {
                _context.Apartments.RemoveRange(building.Apartments);
                _context.Buildings.Remove(building);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingExists(int id)
        {
            return _context.Buildings.Any(e => e.BuildingId == id);
        }
    }
}
