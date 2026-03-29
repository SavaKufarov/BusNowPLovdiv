using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class ArrivalPrediction
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int StopId { get; set; }
        public DateTime EstimatedArrival { get; set; }
        public int DelayMinutes { get; set; }
        public DateTime CalculatedAt { get; set; }

        public Vehicle? Vehicle { get; set; }
        public Stop? Stop { get; set; }
    }
}
