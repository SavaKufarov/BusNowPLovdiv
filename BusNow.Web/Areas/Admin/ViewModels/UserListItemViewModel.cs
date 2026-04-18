namespace BusNow.Web.Areas.Admin.ViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}
