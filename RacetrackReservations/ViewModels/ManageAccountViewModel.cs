using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class ManageAccountViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public required string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int NumberOfLapsWithRaceInstructor { get; set; }
        public int BestTime { get; set; }
    }
}
