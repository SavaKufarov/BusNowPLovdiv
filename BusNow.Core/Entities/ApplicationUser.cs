using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
