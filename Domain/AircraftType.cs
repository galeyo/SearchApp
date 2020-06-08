namespace Domain
{
    public class AircraftType
    {
        public int AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; }
        public int TypeId { get; set; }
        public virtual Type Type { get; set; }
    }
}
