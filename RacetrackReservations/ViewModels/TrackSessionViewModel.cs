using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class TrackSessionViewModel
    {
        public int TrackSessionId { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Max Cars Allowed")]
        public int MaxCarsAllowed { get; set; }

        [Display(Name = "Availible Spots")]
        public int AvailableSpots { get; set; }
    }
}
