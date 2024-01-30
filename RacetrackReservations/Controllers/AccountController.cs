using Microsoft.AspNetCore.Mvc;
using RacetrackReservations.Services;
using RacetrackReservations.ViewModels;
using Microsoft.AspNetCore.Identity;
using RacetrackReservations.Models;
using Microsoft.AspNetCore.Authorization;
using RacetrackReservations.Data;
using System.Security.Claims;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserService _userService;
    private readonly TrackSessionService _trackSessionService;
    private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, UserService userService, TrackSessionService trackSessionService, RacetrackReservationsDbContext racetrackReservationsDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
        _trackSessionService = trackSessionService;
        _racetrackReservationsDbContext = racetrackReservationsDbContext;
    }


    // Redirects to the registration view if the user is not authenticated; otherwise, redirects to the homepage
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            // User is already logged in, redirect to the homepage
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    // Handles the registration process when the registration form is submitted
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _userManager.CreateAsync(new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email, // Using email as the username for simplicity
                    IsAdmin = false,
                    NumberOfLapsWithRaceInstructor = 0,
                    BestTime = 0
                }, model.Password);

                if (result.Succeeded)
                {
                    // Retrieve the registered user
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the same page as after login
                    return RedirectToAction("TrackSessionBrowser", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during registration.");
            }
        }

        // If ModelState is not valid or an error occurred, return to the registration view with validation errors
        return View(model);
    }


    // Redirects to the login view if the user is not authenticated; otherwise, redirects to the homepage
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            // User is already logged in, redirect to the homepage
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    // Handles the login process when the login form is submitted. Adds/removes Claim("IsAdmin", "True") based on IsAdmin value.
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Using PasswordSignInAsync to authenticate the user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var authenticatedUser = await _userManager.FindByEmailAsync(model.Email);

                    // Check if the user is an admin
                    if (authenticatedUser != null && authenticatedUser.IsAdmin)
                    {
                        await _userManager.AddClaimAsync(authenticatedUser, new Claim("IsAdmin", "True"));
                        return RedirectToAction("AdminManagement", "Account");
                    }
                    else
                    {
                        await _userManager.RemoveClaimAsync(authenticatedUser, new Claim("IsAdmin", "True"));
                        return RedirectToAction("TrackSessionBrowser", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
            }
        }

        // If ModelState is not valid or an error occurred, return to the login view with validation errors
        return View(model);
    }


    // Logs out the user and redirects to the home page
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        // Redirect to the home page after logout
        return RedirectToAction("Index", "Home");
    }


    // Displays the admin management view with a list of all users
    [AdminOnly] // This attribute to restricts access to admin users
    [HttpGet]
    public IActionResult AdminManagement()
    {
        var allUsers = _userService.GetAllUsers().ToList();
        var viewModel = new AdminManagementViewModel
        {
            Users = allUsers
        };
        return View(viewModel);
    }


    // Displays the account management view for a specific user
    [AdminOnly]
    [HttpPost]
    [Route("ManageUserData/{userId}")]
    public async Task<IActionResult> ManageUserData(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Map user data to ManageAccountViewModel
                var manageAccountViewModel = new ManageAccountViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    NumberOfLapsWithRaceInstructor = user.NumberOfLapsWithRaceInstructor,
                    BestTime = user.BestTime
                };

                return View(manageAccountViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        catch (Exception)
        {
            return RedirectToAction("Error");
        }
    }


    // Updates user data based on the form submission
    [AdminOnly]
    [HttpPost]
    [Route("UpdateUserData/{userEmail}")]
    public async Task<IActionResult> UpdateUserData(string userEmail, ManageAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = _userService.GetUserDetailsByEmail(userEmail);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.NumberOfLapsWithRaceInstructor = model.NumberOfLapsWithRaceInstructor;
                    user.BestTime = model.BestTime;

                    // Use UserManager to update the user
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Save changes to the database
                        await _racetrackReservationsDbContext.SaveChangesAsync();

                        return RedirectToAction("AdminManagement", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User update failed.");
                        return RedirectToAction("AdminManagement", "Account");
                    }
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the account.");
                return View("Error");
            }
        }
        else
        {
            // If ModelState is not valid or an error occurred, return to the admin management view with validation errors
            return RedirectToAction("AdminManagement", "Account");
        }
    }


    // Deletes a user and associated data (cars, reservations)
    [AdminOnly]
    [HttpPost]
    [Route("Delete/{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Retrieve all cars associated with the user
                var userCars = _racetrackReservationsDbContext.Cars
                    .Where(r => r.UserId == user.Id)
                    .ToList();

                // Delete all cars associated with the user
                _racetrackReservationsDbContext.Cars.RemoveRange(userCars);

                // Retrieve all reservations associated with the user
                var userReservations = _racetrackReservationsDbContext.Reservations
                    .Where(r => r.UserId == user.Id)
                    .ToList();

                // Iterate through reservations to increase available spots for track sessions
                foreach (var reservation in userReservations)
                {
                    var trackSession = _racetrackReservationsDbContext.TrackSessions
                        .FirstOrDefault(ts => ts.TrackSessionId == reservation.TrackSessionId);

                    if (trackSession != null)
                    {
                        // Increase available spots for the track session
                        trackSession.AvailableSpots++;
                    }
                }

                // Delete all reservations associated with the user
                _racetrackReservationsDbContext.Reservations.RemoveRange(userReservations);

                // Remove user from the database
                _racetrackReservationsDbContext.Users.Remove(user);
                _racetrackReservationsDbContext.SaveChanges();

                return RedirectToAction("AdminManagement", "Account");
            }
            else
            {
                return NotFound(new { Message = "User not found." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, new { Message = "Internal Server Error" });
        }
    }


    // Grants admin privileges to a user
    [AdminOnly]
    [HttpPost]
    [Route("MakeAdmin/{userId}")]
    public async Task<IActionResult> MakeAdmin(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.IsAdmin = true;
                _racetrackReservationsDbContext.SaveChanges();
                return RedirectToAction("AdminManagement", "Account");
            }
            else
            {
                return NotFound(new { Message = "User not found." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, new { Message = "Internal Server Error" });
        }
    }


    // Revokes admin privileges from a user
    [AdminOnly]
    [HttpPost]
    [Route("RevokeAdmin/{userId}")]
    public async Task<IActionResult> RevokeAdmin(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.IsAdmin = false;
                _racetrackReservationsDbContext.SaveChanges();
                return RedirectToAction("AdminManagement", "Account");
            }
            else
            {
                return NotFound(new { Message = "User not found." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, new { Message = "Internal Server Error" });
        }
    }


    // Manages reservations for a specific user
    [AdminOnly]
    [HttpPost]
    [Route("ManageUserReservations/{userId}")]
    public async Task<IActionResult> ManageUserReservations(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Retrieve reservations for the current user from the Reservations table
                var userReservations = _racetrackReservationsDbContext.Reservations
                .Where(r => r.UserId == user.Id)
                .Join(
                    _racetrackReservationsDbContext.TrackSessions,
                    reservation => reservation.TrackSessionId,
                    trackSession => trackSession.TrackSessionId,
                    (reservation, trackSession) => new ReservationViewModel
                    {
                        ReservationId = reservation.ReservationId,
                        TrackSessionId = reservation.TrackSessionId,
                        WithInstructor = reservation.WithInstructor,
                        StartTime = trackSession.StartTime,
                        EndTime = trackSession.EndTime,
                        MaxCarsAllowed = trackSession.MaxCarsAllowed,
                        AvailableSpots = trackSession.AvailableSpots
                    }
                )
                .ToList();

                var viewModel = new MyAccountViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Reservations = userReservations
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


    // Displays the track sessions management view
    [AdminOnly]
    [HttpGet]
    public IActionResult ManageTrackSessions()
    {
        return View();
    }


    // Displays the user's account details and reservations
    [HttpGet]
    public IActionResult MyAccount()
    {
        try
        {
            var user = _userService.GetUserDetailsByEmail(User.Identity.Name);

            if (user != null)
            {
                // Retrieve reservations for the current user from the Reservations table
                var userReservations = _racetrackReservationsDbContext.Reservations
                .Where(r => r.UserId == user.Id)
                .Join(
                    _racetrackReservationsDbContext.TrackSessions,
                    reservation => reservation.TrackSessionId,
                    trackSession => trackSession.TrackSessionId,
                    (reservation, trackSession) => new ReservationViewModel
                    {
                        ReservationId = reservation.ReservationId,
                        TrackSessionId = reservation.TrackSessionId,
                        WithInstructor = reservation.WithInstructor,
                        StartTime = trackSession.StartTime,
                        EndTime = trackSession.EndTime,
                        MaxCarsAllowed = trackSession.MaxCarsAllowed,
                        AvailableSpots = trackSession.AvailableSpots
                    }
                )
                .ToList();

                var viewModel = new MyAccountViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    NumberOfLapsWithRaceInstructor = user.NumberOfLapsWithRaceInstructor,
                    BestTime = user.BestTime,
                    Reservations = userReservations
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


    // Displays the account management view for the current user
    public IActionResult ManageAccount()
    {
        try
        {
            // Retrieve the user by email
            var user = _userManager.FindByEmailAsync(User.Identity.Name).Result;

            if (user != null)
            {
                // Map user data to ManageAccountViewModel
                var manageAccountViewModel = new ManageAccountViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                return View(manageAccountViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        catch (Exception)
        {
            return RedirectToAction("Error");
        }
    }


    // Handles updating the account information for the currently logged-in user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAccount(ManageAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Retrieve the user by email
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                if (user != null)
                {
                    // Update user data from the form
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    // Use UserManager to update the user
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Save changes to the database
                        await _racetrackReservationsDbContext.SaveChangesAsync();

                        return RedirectToAction("MyAccount", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User update failed.");
                        return View("ManageAccount", model);
                    }
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the account.");
                return View("Error");
            }
        }
        else
        {
            // If ModelState is not valid or an error occurred, return to the manage account view with validation errors
            return View("ManageAccount", model);
        }
    }


    // Displays available track sessions for users to browse
    public IActionResult TrackSessionBrowser()
    {
        var availableSessions = _trackSessionService.GetAvailableTrackSessions();
        return View(availableSessions);
    }


    // Returns the user details as JSON for API consumption
    [HttpGet("api/user-details")]
    public IActionResult GetUserDetails()
    {
        try
        {
            var userEmail = User.Identity.Name;

            // Use the email to fetch user details from the database
            var userDetails = _userService.GetUserDetailsByEmail(userEmail);

            // Check if the user details are found
            if (userDetails != null)
            {
                // Create a view model to pass user details to the view
                var userViewModel = new UserViewModel
                {
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    NumberOfLapsWithRaceInstructor = userDetails.NumberOfLapsWithRaceInstructor,
                    BestTime = userDetails.BestTime,
                };

                // Pass the userViewModel to the view
                return Ok(userViewModel); // Using Ok to return a 200 status with the user details
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
