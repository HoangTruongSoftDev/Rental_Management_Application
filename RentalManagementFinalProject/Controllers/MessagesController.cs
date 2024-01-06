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
using NuGet.Protocol.Plugins;
using RentalManagementFinalProject.Models;

namespace RentalManagementFinalProject.Controllers
{
    public class MessagesController : Controller
    {
        private readonly RentalManagementDbContext _context;

        public MessagesController(RentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Create");
            }
            else
            {
                var rentalManagementDbContext = _context.Messages.Include(m => m.Recipient).Include(m => m.Sender);
                return View(await rentalManagementDbContext.ToListAsync());
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchType, string searchString, DateTime? searchDateFrom = null, DateTime? searchDateTo = null)
        {
            var sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
            if (string.IsNullOrEmpty(searchString) && (searchType == "Sender" || searchType == "MessageContent"))
            {
                ViewData["ErrorMessage"] = "Missing Search Text";
                var rentalManagementDbContext = _context.Messages.Include(m => m.Recipient).Include(m => m.Sender);
                return View(await rentalManagementDbContext.ToListAsync());
            }
            switch (searchType)
            {
                case "Sender":

                    return View(await _context.Messages.Include(m => m.Recipient).Include(m => m.Sender)
                        .Where(
                        m => m.Recipient.FirstName.ToLower().Trim().Contains(searchString.ToLower().Trim()) || m.Recipient.LastName.ToLower().Trim().Contains(searchString.ToLower().Trim()) ||
                        m.Sender.FirstName.ToLower().Trim().Contains(searchString.ToLower().Trim()) || m.Sender.LastName.ToLower().Trim().Contains(searchString.ToLower().Trim())
                        //m => m.Sender.FirstName.ToLower().Trim().Contains(searchString.ToLower().Trim())
                        )
                        .ToListAsync());
                case "MessageContent":
                    return View(await _context.Messages.Include(m => m.Recipient).Include(m => m.Sender)
                        .Where(m => m.MessageContent.ToLower().Trim().Contains(searchString.ToLower().Trim())).ToListAsync());
                case "TimeCreated":
                    return View(await _context.Messages.Include(m => m.Recipient).Include(m => m.Sender)
                        .Where(m => m.TimeCreated >= searchDateFrom && m.TimeCreated <= searchDateTo).ToListAsync());
                case "All":
                    ViewData["ErrorMessage"] = null;
                    return View(await _context.Messages.Include(m => m.Recipient).Include(m => m.Sender).ToListAsync());
                default:
                    return View(await _context.Messages.Include(m => m.Recipient).Include(m => m.Sender).ToListAsync());
            }
        }
        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {

            if (HttpContext.Session.GetString("User") == null)
            {
                ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && ( u.UserTypeId == 1 || u.UserTypeId == 2)), "UserId", "FirstName");
            }
            else
            {
                var sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                if (sessionUser.UserTypeId == 1)
                {
                    ViewData["listOfRecipientNameManager"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 2), "UserId", "FirstName");
                    ViewData["listOfRecipientNameTenant"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 3), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 2)
                {
                    ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 3 || u.UserTypeId == 1)), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 3)
                {
                    ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 2 || u.UserTypeId == 1)), "UserId", "FirstName");
                }
            }
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,RecipientId,SenderId,MessageContent,TimeCreated")] Models.Message message, int? recipientId = null)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && (u.UserTypeId == 1 || u.UserTypeId == 2)), "UserId", "FirstName");
            }
            else
            {
                var sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                if (sessionUser.UserTypeId == 1)
                {
                    ViewData["listOfRecipientNameManager"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 2), "UserId", "FirstName");
                    ViewData["listOfRecipientNameTenant"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && u.UserTypeId == 3), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 2)
                {
                    ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 3 || u.UserTypeId == 1)), "UserId", "FirstName");
                }
                else if (sessionUser.UserTypeId == 3)
                {
                    ViewData["listOfRecipientName"] = new SelectList(_context.Users.Where(u => u.StatusId != 2 && u.UserId != sessionUser.UserId && (u.UserTypeId == 2 || u.UserTypeId == 1)), "UserId", "FirstName");
                }
            }
            if (string.IsNullOrEmpty(message.MessageContent))
            {
                ViewData["ErrorMessage"] = "Missing Content of Message";
                return View();
            }
            ViewData["ErrorMessage"] = message.RecipientId;
            if (recipientId != null)
            {
                message.RecipientId = (int)recipientId;
            }
            int messageId = _context.Messages.Max(m => m.MessageId) + 1;
            message.MessageId = messageId;
            message.Recipient = _context.Users.SingleOrDefault(currentUser => currentUser.UserId == message.RecipientId);
            message.Sender = _context.Users.SingleOrDefault(currentUser => currentUser.UserId == message.SenderId);
            _context.Add(message);
            await _context.SaveChangesAsync();
            return View();
        }
        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["RecipientId"] = new SelectList(_context.Users, "UserId", "UserId", message.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,RecipientId,SenderId,MessageContent,TimeCreated")] Models.Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            ViewData["RecipientId"] = new SelectList(_context.Users, "UserId", "UserId", message.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
