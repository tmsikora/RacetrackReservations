using Microsoft.AspNetCore.Identity;

namespace RacetrackReservations.Models
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public int NumberOfLapsWithRaceInstructor { get; set; }
        public int BestTime { get; set; }
    }
}
