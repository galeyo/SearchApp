using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<AircraftCategory> AircraftCategories { get; set; }
    }
}
