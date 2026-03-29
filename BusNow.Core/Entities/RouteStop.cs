using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class RouteStop
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int StopId { get; set; }
        public int OrderIndex { get; set; }
        public double DistanceFromStartKm { get; set; }

        public Route Route { get; set; } = null!;
        public Stop Stop { get; set; } = null!;
    }
}
