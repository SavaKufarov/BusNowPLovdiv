using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusNow.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BusNow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServiceAlertsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<TransportHub> _hubContext;

        public ServiceAlertsController(AppDbContext context, IHubContext<TransportHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Admin/ServiceAlerts
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ServiceAlerts.Include(s => s.AffectedLine).Include(s => s.AffectedRoute);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/ServiceAlerts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceAlert = await _context.ServiceAlerts
                .Include(s => s.AffectedLine)
                .Include(s => s.AffectedRoute)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceAlert == null)
            {
                return NotFound();
            }

            return View(serviceAlert);
        }

        // GET: Admin/ServiceAlerts/Create
        public IActionResult Create()
        {
            ViewData["AffectedLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber");
            ViewData["AffectedRouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: Admin/ServiceAlerts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,AffectedLineId,AffectedRouteId,StartDate,EndDate,IsActive")] ServiceAlert serviceAlert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceAlert);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveAlert", new
                {
                    title = serviceAlert.Title,
                    description = serviceAlert.Description,
                    startDate = serviceAlert.StartDate.ToString("dd.MM.yyyy HH:mm"),
                    endDate = serviceAlert.EndDate.ToString("dd.MM.yyyy HH:mm")
                });
                return RedirectToAction(nameof(Index));
            }
            ViewData["AffectedLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", serviceAlert.AffectedLineId);
            ViewData["AffectedRouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", serviceAlert.AffectedRouteId);
            return View(serviceAlert);
        }

        // GET: Admin/ServiceAlerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceAlert = await _context.ServiceAlerts.FindAsync(id);
            if (serviceAlert == null)
            {
                return NotFound();
            }
            ViewData["AffectedLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", serviceAlert.AffectedLineId);
            ViewData["AffectedRouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", serviceAlert.AffectedRouteId);
            return View(serviceAlert);
        }

        // POST: Admin/ServiceAlerts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,AffectedLineId,AffectedRouteId,StartDate,EndDate,IsActive")] ServiceAlert serviceAlert)
        {
            if (id != serviceAlert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceAlert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceAlertExists(serviceAlert.Id))
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
            ViewData["AffectedLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", serviceAlert.AffectedLineId);
            ViewData["AffectedRouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", serviceAlert.AffectedRouteId);
            return View(serviceAlert);
        }

        // GET: Admin/ServiceAlerts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceAlert = await _context.ServiceAlerts
                .Include(s => s.AffectedLine)
                .Include(s => s.AffectedRoute)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceAlert == null)
            {
                return NotFound();
            }

            return View(serviceAlert);
        }

        // POST: Admin/ServiceAlerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceAlert = await _context.ServiceAlerts.FindAsync(id);
            if (serviceAlert != null)
            {
                _context.ServiceAlerts.Remove(serviceAlert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceAlertExists(int id)
        {
            return _context.ServiceAlerts.Any(e => e.Id == id);
        }
    }
}
