namespace RacetrackReservations.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int TrackSessionId { get; set; }
        public string UserId { get; internal set; }
        public string Email { get; set; }
        public bool WithInstructor { get; set; }
    }
}
