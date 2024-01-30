namespace RacetrackReservations.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public required string Brand { get; set; }
        public required string ModelName { get; set; }
        public int Year { get; set; }
        public bool HasRollcage { get; set; }
        public bool HasFireExtinguisher { get; set; }

        public string UserId { get; set; }
    }
}
