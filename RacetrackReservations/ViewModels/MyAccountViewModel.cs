using RacetrackReservations.Models;

namespace RacetrackReservations.ViewModels
{
    public class MyAccountViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int NumberOfLapsWithRaceInstructor {  get; set; }
        public int BestTime { get; set; }
        public List<ReservationViewModel> Reservations { get; set; }
        public List<Car> Cars { get; set; }
    }
}
