using System.ComponentModel.DataAnnotations;

namespace RacetrackReservations.ViewModels
{
    public class CarViewModel
    {
        public int CarId { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Brand is required.")]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Model is required.")]
        public string ModelName { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }

        [Display(Name = "Rollcage")]
        public bool HasRollcage { get; set; }

        [Display(Name = "Fire extinguisher")]
        public bool HasFireExtinguisher { get; set; }

        public string UserId { get; set; }
    }
}
