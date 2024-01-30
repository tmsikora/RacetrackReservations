using Microsoft.AspNetCore.Identity;
using RacetrackReservations.Data;
using RacetrackReservations.Models;
using RacetrackReservations.ViewModels;

namespace RacetrackReservations.Services
{
    public class UserServiceImpl : UserService
    {
        // Sample data to simulate a database
        private static readonly List<UserViewModel> Users = new List<UserViewModel>();
        private static readonly List<CarViewModel> Cars = new List<CarViewModel>();

        private readonly UserManager<User> _userManager;
        private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;

        public UserServiceImpl(UserManager<User> userManager, RacetrackReservationsDbContext racetrackReservationsDbContext)
        {
            _userManager = userManager;
            _racetrackReservationsDbContext = racetrackReservationsDbContext;
        }


        // Registers a new user in the simulated database
        public void RegisterUser(RegisterViewModel model)
        {
            // Add a new user to the database
            var newUser = new UserViewModel
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
            };
            Users.Add(newUser);
        }


        // Retrieves account management details for a user
        public ManageAccountViewModel GetManageAccountViewModel(string userEmail)
        {
            // Retrieve user data for account management
            var user = Users.Find(u => u.Email == userEmail);
            if (user != null)
            {
                return new ManageAccountViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }
            return null;
        }


        // Retrieves all users from the Identity database
        public IEnumerable<User> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }


        // Retrieves cars associated with a user from the simulated database
        public IEnumerable<CarViewModel> GetUserCars(string userId)
        {
            // Retrieve user's cars from the database
            return Cars.FindAll(c => c.UserId == userId);
        }


        // Retrieves user details by email from the Identity database
        public User GetUserDetailsByEmail(string email)
        {
            return _racetrackReservationsDbContext.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
