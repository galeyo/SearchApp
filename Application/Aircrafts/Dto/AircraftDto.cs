using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Aircrafts.Dto
{
    public class AircraftDto
    {
        public string AircraftName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public ICollection<string> Categories { get; set; }
        public ICollection<string> Types { get; set; }
    }
}
