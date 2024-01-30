using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Number of laps with Race Instructor")]
        public int NumberOfLapsWithRaceInstructor { get; set; }

        [Display(Name = "Best Time")]
        public int BestTime { get; set; }
    }
}
