using System;
using System.Collections.Generic;

namespace Domain
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string AircraftName { get; set; }
        public Int16 YearInService { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public virtual ICollection<AircraftCategory> AircraftCategory { get; set; }
        public virtual ICollection<AircraftType> AircraftTypes { get; set; }
    }
}
