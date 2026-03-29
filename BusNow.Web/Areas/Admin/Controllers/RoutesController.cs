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
    public class RoutesController : Controller
    {
        private readonly AppDbContext _context;

        public RoutesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Routes.Include(r => r.TransportLine);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var route = await _context.Routes
                .Include(r => r.TransportLine)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (route == null) return NotFound();

            return View(route);
        }

        public IActionResult Create()
        {
            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransportLineId,Name,Direction,GeoJson,IsTemporary")] Core.Entities.Route route)
        {
            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", route.TransportLineId);
            return View(route);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var route = await _context.Routes.FindAsync(id);
            if (route == null) return NotFound();

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", route.TransportLineId);
            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TransportLineId,Name,Direction,GeoJson,IsTemporary")] Core.Entities.Route route)
        {
            if (id != route.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportLineId"] = new SelectList(_context.TransportLines.OrderBy(x => x.LineNumber), "Id", "LineNumber", route.TransportLineId);
            return View(route);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var route = await _context.Routes
                .Include(r => r.TransportLine)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (route == null) return NotFound();

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route != null)
            {
                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.Id == id);
        }
    }
}