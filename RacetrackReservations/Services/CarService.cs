using RacetrackReservations.ViewModels;

namespace RacetrackReservations.Services
{
    public interface CarService
    {
        void AddCar(CarViewModel model, string userId);
        CarViewModel GetCarForEdit(int carId);
        void UpdateCar(CarViewModel model);
        void DeleteCar(int carId);
    }
}
