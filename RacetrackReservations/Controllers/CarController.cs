using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RacetrackReservations.Data;
using RacetrackReservations.Models;
using RacetrackReservations.Services;
using RacetrackReservations.ViewModels;

namespace RacetrackReservations.Controllers
{
    [Authorize]
    public class CarController : Controller
    {
        private readonly CarService _carService;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;

        public CarController(CarService carService, UserManager<User> userManager, UserService userService, RacetrackReservationsDbContext racetrackReservationsDbContext)
        {
            _carService = carService;
            _userManager = userManager;
            _userService = userService;
            _racetrackReservationsDbContext = racetrackReservationsDbContext;
        }


        // Displays the user's cars and related information
        public async Task<IActionResult> ManageCars(string userId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the logged-in user is the same as the requested userId
                if (currentUser == null || currentUser.Id != userId)
                {
                    // Unauthorized access, redirect to the homepage
                    return RedirectToAction("Index", "Home");
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Retrieve cars for the current user from the Cars table
                    var userCars = _racetrackReservationsDbContext.Cars
                    .Where(r => r.UserId == user.Id)
                    .ToList();

                    Console.WriteLine("Cars in databse");
                    foreach (var car in userCars)
                    {
                        Console.WriteLine($"Car ID: {car.CarId}, Brand: {car.Brand}, Model: {car.ModelName}, Year: {car.Year}");
                    }

                    var viewModel = new MyAccountViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Cars = userCars
                    };

                    return View(viewModel);
                }
                else
                {
                    return NotFound(new { Message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return View("Error");
            }
        }


        // Displays the view for adding a new car
        public IActionResult AddCar()
        {
            return View();
        }


        // Handles the submission of the new car form
        [HttpPost]
        public IActionResult AddNewCar(CarViewModel model)
        {
            try
            {
                var userTemp = _userService.GetUserDetailsByEmail(model.UserId);

                if (ModelState.IsValid)
                {
                    _carService.AddCar(model, userTemp.Id);
                    return RedirectToAction("ManageCars", new { userId = userTemp.Id });
                }

                // If ModelState is not valid, return to the add view with errors
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Exception: {ex.Message}");
                return View("Error");
            }
        }


        // Displays the view for editing a specific car
        public IActionResult EditCar(int carId)
        {
            // Retrieve car data and populate the CarViewModel for editing
            var carViewModel = _carService.GetCarForEdit(carId);
            
            // Check if the logged-in user is the owner of the car
            if (carViewModel != null && carViewModel.UserId != _userManager.GetUserId(User))
            {
                // Unauthorized access, redirect to the homepage
                return RedirectToAction("Index", "Home");
            }

            return View(carViewModel);
        }


        // Handles the submission of the updated car form
        [HttpPost]
        public IActionResult UpdateCar(CarViewModel model)
        {
            // Check if the logged-in user is the owner of the car
            if (model != null && model.UserId != _userManager.GetUserId(User))
            {
                // Unauthorized access, redirect to the homepage
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _carService.UpdateCar(model);
                return RedirectToAction("ManageCars", new { userId = model.UserId });
            }

            // If ModelState is not valid, return to the edit car view with errors
            return View("EditCar", model);
        }


        // Handles the deletion of a specific car
        public IActionResult DeleteCar(int carId, string userId)
        {
            _carService.DeleteCar(carId);
            return RedirectToAction("ManageCars", new { userId });
        }
    }
}
