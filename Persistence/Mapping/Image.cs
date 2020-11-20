using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Mapping
{
    [DelimitedRecord("|")]
    public class Image
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string ImageUrl { get; set; }
    }
}
