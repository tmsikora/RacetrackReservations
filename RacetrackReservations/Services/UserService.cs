using RacetrackReservations.Models;
using RacetrackReservations.ViewModels;

namespace RacetrackReservations.Services
{
    public interface UserService
    {
        void RegisterUser(RegisterViewModel model);
        ManageAccountViewModel GetManageAccountViewModel(string userEmail);
        IEnumerable<User> GetAllUsers();
        IEnumerable<CarViewModel> GetUserCars(string userId);
        User GetUserDetailsByEmail(string email);
    }
}
