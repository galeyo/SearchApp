using System.Collections.Generic;

namespace Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<AircraftCategory> AircraftCategories { get; set; }
    }
}
