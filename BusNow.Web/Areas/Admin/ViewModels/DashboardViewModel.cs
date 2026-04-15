namespace BusNow.Web.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public int LinesCount { get; set; }
        public int StopsCount { get; set; }
        public int RoutesCount { get; set; }
        public int ActiveVehiclesCount { get; set; }
        public int SchedulesCount { get; set; }
        public int ActiveAlertsCount { get; set; }
    }
}
