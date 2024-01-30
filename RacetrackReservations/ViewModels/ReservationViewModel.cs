using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public int TrackSessionId { get; set; }
        public string UserId { get; internal set; }
        public string Email { get; set; }
        public bool WithInstructor { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCarsAllowed { get; set; }
        public int AvailableSpots { get; set; }
    }
}
