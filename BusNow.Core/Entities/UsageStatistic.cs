using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class UsageStatistic
    {
        public int Id { get; set; }
        public string PageName { get; set; } = null!;
        public DateTime VisitedAt { get; set; }
        public string? UserId { get; set; }
    }
}
