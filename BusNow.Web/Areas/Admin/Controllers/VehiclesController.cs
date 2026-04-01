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
    public class VehiclesController : Controller
    {
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Vehicles
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Vehicles.Include(v => v.Route).Include(v => v.TransportLine);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Route)
                .Include(v => v.TransportLine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Admin/Vehicles/Create
        public IActionResult Create()
        {
            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber");
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: Admin/Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransportLineId,RouteId,CurrentStopOrderIndex,RegistrationNumber,VehicleType,CurrentLatitude,CurrentLongitude,LastUpdateTime,IsActive")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                vehicle.LastUpdateTime = DateTime.Now;

                if (vehicle.RouteId.HasValue)
                {
                    var firstRouteStop = await _context.RouteStops
                        .Include(rs => rs.Stop)
                        .Where(rs => rs.RouteId == vehicle.RouteId.Value)
                        .OrderBy(rs => rs.OrderIndex)
                        .FirstOrDefaultAsync();

                    if (firstRouteStop?.Stop != null)
                    {
                        vehicle.CurrentStopOrderIndex = firstRouteStop.OrderIndex;
                        vehicle.CurrentLatitude = firstRouteStop.Stop.Latitude;
                        vehicle.CurrentLongitude = firstRouteStop.Stop.Longitude;
                    }
                }

                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", vehicle.TransportLineId);
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", vehicle.RouteId);
            return View(vehicle);
        }

        // GET: Admin/Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber");
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name");
            return View(vehicle);
        }

        // POST: Admin/Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TransportLineId,RouteId,CurrentStopOrderIndex,RegistrationNumber,VehicleType,CurrentLatitude,CurrentLongitude,LastUpdateTime,IsActive")] Vehicle vehicle)
        {
            if (id != vehicle.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

                    if (existingVehicle == null)
                        return NotFound();

                    if (existingVehicle.RouteId != vehicle.RouteId && vehicle.RouteId.HasValue)
                    {
                        var firstRouteStop = await _context.RouteStops
                            .Include(rs => rs.Stop)
                            .Where(rs => rs.RouteId == vehicle.RouteId.Value)
                            .OrderBy(rs => rs.OrderIndex)
                            .FirstOrDefaultAsync();

                        if (firstRouteStop?.Stop != null)
                        {
                            vehicle.CurrentStopOrderIndex = firstRouteStop.OrderIndex;
                            vehicle.CurrentLatitude = firstRouteStop.Stop.Latitude;
                            vehicle.CurrentLongitude = firstRouteStop.Stop.Longitude;
                        }
                    }

                    vehicle.LastUpdateTime = DateTime.Now;

                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vehicles.Any(e => e.Id == vehicle.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", vehicle.TransportLineId);
            ViewData["RouteId"] = new SelectList(_context.Routes.OrderBy(x => x.Name), "Id", "Name", vehicle.RouteId);
            return View(vehicle);
        }

        // GET: Admin/Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Route)
                .Include(v => v.TransportLine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Admin/Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
