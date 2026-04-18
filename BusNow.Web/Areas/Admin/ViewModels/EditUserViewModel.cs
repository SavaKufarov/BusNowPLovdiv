using System.ComponentModel.DataAnnotations;

namespace BusNow.Web.Areas.Admin.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "Име")]
    public string? FullName { get; set; }

    [Display(Name = "Имейл")]
    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Потребителско име")]
    public string? UserName { get; set; }

    [Display(Name = "Администратор")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Активен")]
    public bool IsActive { get; set; }
}