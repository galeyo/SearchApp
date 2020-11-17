using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Mapping
{
    [DelimitedRecord("|")]
    public class AircraftType
    {
        public int AircraftId { get; set; }
        public int TypeId { get; set; }
    }
}
