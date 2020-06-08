using System.Collections.Generic;

namespace Application.Aircrafts.Dto
{
    public class AircraftEnvelope
    {
        public ICollection<AircraftDto> Aircrafts { get; set; }
        public int Count { get; set; }
    }
}
