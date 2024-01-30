using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RacetrackReservations.Data;
using RacetrackReservations.Models;
using RacetrackReservations.ViewModels;

[Route("Account/Reservation")]
public class ReservationController : Controller
{
    private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;
    private readonly UserManager<User> _userManager;

    public ReservationController(UserManager<User> userManager, RacetrackReservationsDbContext racetrackReservationsDbContext)
    {
        _racetrackReservationsDbContext = racetrackReservationsDbContext;
        _userManager = userManager;
    }


    // Displays the reservation view for a specific track session
    [HttpGet]
    public IActionResult Index(int trackSessionId)
    {
        return View("~/Views/Account/Reservation.cshtml");
    }


    // Handles the confirmation of a new reservation
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("ConfirmReservation")]
    public async Task<IActionResult> ConfirmReservationAsync(ReservationViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Fetch user details using the service
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var userId = await _userManager.GetUserIdAsync(user);

                // Check if the user is found
                if (user != null)
                {
                    // Create a new reservation
                    var reservation = new Reservation
                    {
                        TrackSessionId = model.TrackSessionId,
                        Email = model.Email,
                        WithInstructor = model.WithInstructor,
                        UserId = userId
                    };

                    // Add the reservation to the database and save changes
                    _racetrackReservationsDbContext.Reservations.Add(reservation);
                    _racetrackReservationsDbContext.SaveChanges();

                    // Retrieve the track session from the database, where TrackSessionId == model.TrackSessionId
                    var trackSession = _racetrackReservationsDbContext.TrackSessions
                        .FirstOrDefault(ts => ts.TrackSessionId == model.TrackSessionId);

                    if (trackSession != null)
                    {
                        // Decrease available spots for the track session
                        trackSession.AvailableSpots--;

                        // Save changes to both the reservation and TrackSession entities
                        _racetrackReservationsDbContext.SaveChanges();

                        return RedirectToAction("TrackSessionBrowser", "Account");
                    }
                }
                else
                {
                    return NotFound(new { Message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
        return BadRequest(new { Message = "Invalid data or unexpected error." });
    }


    // Handles the deletion of a specific reservation
    [HttpPost]
    [Route("Delete/{reservationId}")]
    public IActionResult Delete(int reservationId)
    {
        try
        {
            var reservation = _racetrackReservationsDbContext.Reservations.Find(reservationId);

            if (reservation != null)
            {
                // Retrieve track session associated with the reservation
                var trackSession = _racetrackReservationsDbContext.TrackSessions.Find(reservation.TrackSessionId);

                if (trackSession != null)
                {
                    // Increase available spots for the track session
                    trackSession.AvailableSpots++;
                }

                // Remove the reservation from the database and save changes
                _racetrackReservationsDbContext.Reservations.Remove(reservation);
                _racetrackReservationsDbContext.SaveChanges();

                return RedirectToAction("MyAccount", "Account");
            }
            else
            {
                return NotFound(new { Message = "Reservation not found." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, new { Message = "Internal Server Error" });
        }
    }


    // Retrieves user reservations for the track session browser fetch
    [HttpGet("UserReservations")]
    public async Task<IActionResult> GetUserReservations()
    {
        try
        {
            // Fetch the current user
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (user != null)
            {
                // Fetch reservations for the current user
                var userReservations = _racetrackReservationsDbContext.Reservations
                    .Where(r => r.UserId == user.Id)
                    .Select(r => new
                    {
                        r.ReservationId,
                        r.TrackSessionId,
                        r.Email,
                        r.WithInstructor
                    })
                    .ToList();

                return Ok(userReservations);
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
}
