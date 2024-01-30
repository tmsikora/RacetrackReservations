using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class ManageTrackSessionsViewModel
    {
        [Display(Name = "Select Day")]
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; }
    }
}
