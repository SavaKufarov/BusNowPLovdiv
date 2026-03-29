using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusNow.Core.Entities;
using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace BusNow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TransportLinesController : Controller
    {
        private readonly AppDbContext _context;

        public TransportLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TransportLines
        public async Task<IActionResult> Index()
        {
            return View(await _context.TransportLines.ToListAsync());
        }

        // GET: Admin/TransportLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportLine = await _context.TransportLines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transportLine == null)
            {
                return NotFound();
            }

            return View(transportLine);
        }

        // GET: Admin/TransportLines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TransportLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LineNumber,Type,DirectionA,DirectionB,IsActive")] TransportLine transportLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transportLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transportLine);
        }

        // GET: Admin/TransportLines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportLine = await _context.TransportLines.FindAsync(id);
            if (transportLine == null)
            {
                return NotFound();
            }
            return View(transportLine);
        }

        // POST: Admin/TransportLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LineNumber,Type,DirectionA,DirectionB,IsActive")] TransportLine transportLine)
        {
            if (id != transportLine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transportLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransportLineExists(transportLine.Id))
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
            return View(transportLine);
        }

        // GET: Admin/TransportLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportLine = await _context.TransportLines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transportLine == null)
            {
                return NotFound();
            }

            return View(transportLine);
        }

        // POST: Admin/TransportLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transportLine = await _context.TransportLines.FindAsync(id);
            if (transportLine != null)
            {
                _context.TransportLines.Remove(transportLine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransportLineExists(int id)
        {
            return _context.TransportLines.Any(e => e.Id == id);
        }
    }
}
