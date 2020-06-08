using System.Collections.Generic;

namespace Domain
{
    public class Type
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public virtual ICollection<AircraftType> AircraftTypes { get; set; }
    }
}
