using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusNow.Core.Entities;

namespace BusNow.Web.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FavoritesController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var favoriteStops = await _context.FavoriteStops
            .Include(x => x.Stop)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        var favoriteLines = await _context.FavoriteLines
            .Include(x => x.Line)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        ViewBag.FavoriteStops = favoriteStops;
        ViewBag.FavoriteLines = favoriteLines;

        return View();
    }

    public async Task<IActionResult> AddStop(int stopId)
    {
        var userId = _userManager.GetUserId(User);

        var exists = await _context.FavoriteStops
            .AnyAsync(x => x.UserId == userId && x.StopId == stopId);

        if (!exists)
        {
            _context.FavoriteStops.Add(new FavoriteStop
            {
                UserId = userId!,
                StopId = stopId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddLine(int lineId)
    {
        var userId = _userManager.GetUserId(User);

        var exists = await _context.FavoriteLines
            .AnyAsync(x => x.UserId == userId && x.LineId == lineId);

        if (!exists)
        {
            _context.FavoriteLines.Add(new FavoriteLine
            {
                UserId = userId!,
                LineId = lineId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveStop(int id)
    {
        var userId = _userManager.GetUserId(User);

        var favorite = await _context.FavoriteStops
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (favorite != null)
        {
            _context.FavoriteStops.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveLine(int id)
    {
        var userId = _userManager.GetUserId(User);

        var favorite = await _context.FavoriteLines
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (favorite != null)
        {
            _context.FavoriteLines.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}