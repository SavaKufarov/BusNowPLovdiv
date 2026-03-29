using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public int TransportLineId { get; set; }
        public string Name { get; set; } = null!;
        public string Direction { get; set; } = null!;
        public string? GeoJson { get; set; }
        public bool IsTemporary { get; set; }

        public TransportLine? TransportLine { get; set; }
        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
