using RacetrackReservations.Data;
using RacetrackReservations.Models;
using RacetrackReservations.ViewModels;

namespace RacetrackReservations.Services
{
    public class CarServiceImpl : CarService
    {
        private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;

        public CarServiceImpl(RacetrackReservationsDbContext racetrackReservationsDbContext)
        {
            _racetrackReservationsDbContext = racetrackReservationsDbContext;
        }


        // Adds a new car to the database
        public void AddCar(CarViewModel model, string userId)
        {
            var newCar = new Car
            {
                Brand = model.Brand,
                ModelName = model.ModelName,
                Year = model.Year,
                HasRollcage = model.HasRollcage,
                HasFireExtinguisher = model.HasFireExtinguisher,
                UserId = userId
            };

            _racetrackReservationsDbContext.Cars.Add(newCar);
            _racetrackReservationsDbContext.SaveChanges();
        }


        // Retrieves car details for editing
        public CarViewModel GetCarForEdit(int carId)
        {
            var car = _racetrackReservationsDbContext.Cars.Find(carId);

            if (car != null)
            {
                return new CarViewModel
                {
                    CarId = car.CarId,
                    Brand = car.Brand,
                    ModelName = car.ModelName,
                    Year = car.Year,
                    HasRollcage = car.HasRollcage,
                    HasFireExtinguisher = car.HasFireExtinguisher,
                    UserId = car.UserId
                };
            }

            return null;
        }


        // Updates an existing car in the database
        public void UpdateCar(CarViewModel model)
        {
            var existingCar = _racetrackReservationsDbContext.Cars.Find(model.CarId);

            if (existingCar != null)
            {
                existingCar.Brand = model.Brand;
                existingCar.ModelName = model.ModelName;
                existingCar.Year = model.Year;
                existingCar.HasRollcage = model.HasRollcage;
                existingCar.HasFireExtinguisher = model.HasFireExtinguisher;

                _racetrackReservationsDbContext.SaveChanges();
            }
        }


        // Deletes a car from the database
        public void DeleteCar(int carId)
        {
            var carToDelete = _racetrackReservationsDbContext.Cars.Find(carId);

            if (carToDelete != null)
            {
                _racetrackReservationsDbContext.Cars.Remove(carToDelete);
                _racetrackReservationsDbContext.SaveChanges();
            }
        }
    }
}
