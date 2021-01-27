using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Propmaster.Areas.Identity.Data;
using Propmaster.Data;
using Propmaster.Models;

namespace Propmaster.Views
{
    public class BookingController : Controller
    {
        private readonly PropmasterModelContext _context;
        private readonly UserManager<PropmasterUser> _userManager;
        IConfiguration configuration;
        static IQueueClient queueClient;

        public BookingController(PropmasterModelContext context, UserManager<PropmasterUser> userManager, IConfiguration iConfig)
        {
            _context = context;
            _userManager = userManager;
            configuration = iConfig;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            PropmasterUser user = await _userManager.GetUserAsync(this.User);
            List<Booking> bookings = new List<Booking>();
            if (user.Type == "Agent")
            {
                ViewData["Title"] = "View bookings";
                bookings = await _context.Booking.ToListAsync();
            }
            else if (user.Type == "Client")
            {
                ViewData["Title"] = "My bookings";
                bookings = await _context.Booking.Where(b => b.ClientId == user.Id).ToListAsync();
            }
            return View(bookings);
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create(string propertyId, string type)
        {
            PropmasterUser user = await _userManager.GetUserAsync(this.User);
            return View(new Booking { ClientId = user.Id, PropertyId = propertyId, Status="Unassigned", Type = type, ReservedDate = DateTime.Today.AddDays(1)});
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,ClientId,ReservedDate,PropertyId,AgentId,Status,Type,Remarks")] Booking booking)
        {
            // Create a new message to send to the queue.
            var msg = new Message();
            string json = JsonConvert.SerializeObject(booking);
            byte[] arr = Encoding.ASCII.GetBytes(json);
            msg.ContentType = "application/json";
            msg.Body = arr;

            // Write the body of the message to the console.
            Debug.WriteLine($"Sending message: {json}");

            // Send the message to the queue.
            queueClient = new QueueClient(configuration["QueueConnectionString"], configuration["QueueName"]);
            await queueClient.SendAsync(msg);

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,ClientId,ReservedDate,PropertyId,AgentId,Status,Type,Remarks")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancel(int id)
        {
            var agent = await _userManager.GetUserAsync(this.User);
            var booking = await _context.Booking.FindAsync(id);
            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            //Display successfully accepted message
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Accept/5
        public async Task<IActionResult> Accept(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Accept/5
        [HttpPost, ActionName("Accept")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAccept(int id)
        {
            var agent = await _userManager.GetUserAsync(this.User);
            var booking = await _context.Booking.FindAsync(id);
            booking.AgentId = agent.Id;
            booking.Status = "Assigned";
            await _context.SaveChangesAsync();
            //Display successfully accepted message
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
