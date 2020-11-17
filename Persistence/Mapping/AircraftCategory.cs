using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Mapping
{
    [DelimitedRecord("|")]
    public class AircraftCategory
    {
        public int AircraftId { get; set; }
        public int CategoryId { get; set; }
    }
}
