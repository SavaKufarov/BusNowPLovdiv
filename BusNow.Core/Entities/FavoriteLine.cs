using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class FavoriteLine
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int LineId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public TransportLine Line { get; set; } = null!;
    }
}
