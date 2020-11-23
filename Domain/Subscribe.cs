using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Subscribe
    {
        public int AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
