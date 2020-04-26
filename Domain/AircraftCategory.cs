using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class AircraftCategory
    {
        public int AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
