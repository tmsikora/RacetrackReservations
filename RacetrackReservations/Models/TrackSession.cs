namespace RacetrackReservations.Models
{
    public class TrackSession
    {
        public int TrackSessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCarsAllowed { get; set; }
        public int AvailableSpots { get; set; }
    }
}
