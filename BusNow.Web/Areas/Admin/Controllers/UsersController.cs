using BusNow.Core.Entities;
using BusNow.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusNow.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var model = new List<UserListItemViewModel>();

        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            model.Add(new UserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                IsAdmin = isAdmin,
                IsActive = user.IsActive
            });
        }

        return View(model);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.UserName,
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
            IsActive = user.IsActive
        };

        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.UserName,
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
            IsActive = user.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.UserName;
        user.IsActive = model.IsActive;
        user.IsAdmin = model.IsAdmin;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var isCurrentlyAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (model.IsAdmin && !isCurrentlyAdmin)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else if (!model.IsAdmin && isCurrentlyAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.UserName,
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
            IsActive = user.IsActive
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }
}