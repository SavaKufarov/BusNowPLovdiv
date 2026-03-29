using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BusNow.Infrastructure.Data;
using BusNow.Core.Entities;

namespace BusNow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SchedulesController : Controller
    {
        private readonly AppDbContext _context;

        public SchedulesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Schedules
                .Include(s => s.TransportLine)
                .Include(s => s.Route);

            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.TransportLine)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        public IActionResult Create()
        {
            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber");
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransportLineId,RouteId,DepartureTime,IsWeekend,IsHoliday")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", schedule.TransportLineId);
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", schedule.RouteId);
            return View(schedule);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", schedule.TransportLineId);
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", schedule.RouteId);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TransportLineId,RouteId,DepartureTime,IsWeekend,IsHoliday")] Schedule schedule)
        {
            if (id != schedule.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", schedule.TransportLineId);
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", schedule.RouteId);
            return View(schedule);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.TransportLine)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }
    }
}