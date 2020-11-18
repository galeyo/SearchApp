using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Aircraft
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string AircraftName { get; set; }
        public Int16 YearInService { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public virtual ICollection<AircraftCategory> AircraftCategory { get; set; }
        public virtual ICollection<AircraftType> AircraftTypes { get; set; }
        public string Country { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
