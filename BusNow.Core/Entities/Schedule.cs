using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public int TransportLineId { get; set; }
        public int? RouteId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }

        public TransportLine? TransportLine { get; set; }
        public Route? Route { get; set; }
    }
}
