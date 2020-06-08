using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Aircrafts.Dto
{
    public class AircraftDto
    {
        public int Id { get; set; }
        [Required]
        public string AircraftName { get; set; }
        [Required]
        public Int16 YearInService { get; set; }
        [Required]
        public string Description { get; set; }
        public string Image { get; set; }
        public ICollection<string> Categories { get; set; }
        public ICollection<string> Types { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
