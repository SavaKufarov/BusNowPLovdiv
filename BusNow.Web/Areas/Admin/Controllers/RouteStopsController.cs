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

namespace BusNow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RouteStopsController : Controller
    {
        private readonly AppDbContext _context;

        public RouteStopsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/RouteStops
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.RouteStops.Include(r => r.Route).Include(r => r.Stop);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/RouteStops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStops
                .Include(r => r.Route)
                .Include(r => r.Stop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // GET: Admin/RouteStops/Create
        public IActionResult Create()
        {
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name");
            ViewData["StopId"] = new SelectList(_context.Stops.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: Admin/RouteStops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RouteId,StopId,OrderIndex,DistanceFromStartKm")] RouteStop routeStop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeStop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", routeStop.RouteId);
            ViewData["StopId"] = new SelectList(_context.Stops.OrderBy(x => x.Name), "Id", "Name", routeStop.StopId);
            return View(routeStop);
        }

        // GET: Admin/RouteStops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStops.FindAsync(id);
            if (routeStop == null)
            {
                return NotFound();
            }
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", routeStop.RouteId);
            ViewData["StopId"] = new SelectList(_context.Stops.OrderBy(x => x.Name), "Id", "Name", routeStop.StopId);
            return View(routeStop);
        }

        // POST: Admin/RouteStops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RouteId,StopId,OrderIndex,DistanceFromStartKm")] RouteStop routeStop)
        {
            if (id != routeStop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteStopExists(routeStop.Id))
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
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", routeStop.RouteId);
            ViewData["StopId"] = new SelectList(_context.Stops.OrderBy(x => x.Name), "Id", "Name", routeStop.StopId);
            return View(routeStop);
        }

        // GET: Admin/RouteStops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStops
                .Include(r => r.Route)
                .Include(r => r.Stop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // POST: Admin/RouteStops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeStop = await _context.RouteStops.FindAsync(id);
            if (routeStop != null)
            {
                _context.RouteStops.Remove(routeStop);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RouteStopExists(int id)
        {
            return _context.RouteStops.Any(e => e.Id == id);
        }
    }
}
