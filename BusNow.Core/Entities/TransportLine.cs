using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class TransportLine
    {
        public int Id { get; set; }
        public string LineNumber { get; set; } = null!;
        public string Type { get; set; } = null!; // Автобус, Маршрутка
        public string DirectionA { get; set; } = null!;
        public string DirectionB { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<FavoriteLine> FavoriteLines { get; set; } = new List<FavoriteLine>();
    }
}
