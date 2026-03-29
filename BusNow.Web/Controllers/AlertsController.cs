using BusNow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Controllers;

public class AlertsController : Controller
{
    private readonly AppDbContext _context;

    public AlertsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;

        var alerts = await _context.ServiceAlerts
            .Where(x => x.IsActive && x.StartDate <= now && x.EndDate >= now)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync();

        return View(alerts);
    }
}