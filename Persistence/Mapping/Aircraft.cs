using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Mapping
{
    [DelimitedRecord("|")]
    public class Aircraft
    {
        public int Id { get; set; }
        public string AircraftName { get; set; }
        public Int16 YearInService { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }
    }
}
