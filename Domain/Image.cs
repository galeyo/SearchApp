using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Image
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; }
        public string ImageUrl { get; set; }
    }
}
