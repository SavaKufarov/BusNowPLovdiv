using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int TransportLineId { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public string VehicleType { get; set; } = null!;
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsActive { get; set; } = true;

        public TransportLine? TransportLine { get; set; } = null!;
        public ICollection<ArrivalPrediction> ArrivalPredictions { get; set; } = new List<ArrivalPrediction>();
    }
}
