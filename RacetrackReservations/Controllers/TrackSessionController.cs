using Microsoft.AspNetCore.Mvc;
using RacetrackReservations.Models;
using RacetrackReservations.Services;
using RacetrackReservations.ViewModels;

[Route("api/track-sessions")]
public class TrackSessionController : Controller
{
    private readonly TrackSessionService _trackSessionService;

    public TrackSessionController(TrackSessionService trackSessionService)
    {
        _trackSessionService = trackSessionService;
    }


    // Displays the available track sessions in the track session browser
    public IActionResult TrackSessionBrowser()
    {
        try
        {
            // Retrieve track sessions using the service
            var availableTrackSessions = _trackSessionService.GetAvailableTrackSessions();

            // Pass the track sessions to the view
            return View(availableTrackSessions);
        }
        catch (Exception)
        {
            return RedirectToAction("Error");
        }
    }


    // Retrieves track sessions for a specific day
    [HttpGet("for-day")]
    public IActionResult GetTrackSessionsForDay([FromQuery] DateTime selectedDate)
    {
        try
        {
            var trackSessions = _trackSessionService.GetTrackSessionsForDay(selectedDate);
            return Ok(trackSessions);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    // Retrieves details of a specific track session
    [HttpGet("details/{id}")]
    public IActionResult GetTrackSessionDetails(int id)
    {
        try
        {
            var trackSession = _trackSessionService.GetTrackSessionById(id);

            if (trackSession == null)
            {
                return NotFound();
            }

            return Ok(trackSession);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    // Displays the view for managing track sessions
    [HttpGet]
    public IActionResult ManageTrackSessions()
    {
        return View();
    }


    // Generates and adds track sessions to the database based on the selected date
    [HttpPost]
    public IActionResult GenerateTrackSessions(ManageTrackSessionsViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Generate track sessions for the selected date
                DateTime selectedDate = model.SelectedDate;
                List<TrackSession> generatedSessions = GenerateTrackSessionsInternal(selectedDate);

                // Add the generated track sessions to the database
                _trackSessionService.AddTrackSessions(generatedSessions);

                return RedirectToAction("ManageTrackSessions", "Account");
            }
            else
            {
                return View("~/Views/Account/ManageTrackSessions.cshtml", model);
            }
        }
        catch (Exception)
        {
            return RedirectToAction("Error");
        }
    }


    // Internal method to generate track sessions based on the chosen day
    private List<TrackSession> GenerateTrackSessionsInternal(DateTime chosenDay)
    {
        // Set the start and end times for track sessions
        DateTime startTime = chosenDay.Date.AddHours(9);  // First session starts at 09:00
        DateTime endTime = chosenDay.Date.AddHours(17);   // Last session ends at 17:00

        // Initialize a list to store the generated track sessions
        List<TrackSession> trackSessions = new List<TrackSession>();

        // Generate track sessions every 2 hours until the end time
        while (startTime.AddHours(2) <= endTime)
        {
            TrackSession trackSession = new TrackSession
            {
                StartTime = startTime,
                EndTime = startTime.AddHours(2),
                MaxCarsAllowed = 10,    // Set the maximum number of cars allowed for each session
                AvailableSpots = 10     // Set available spots for each session
            };

            // Add the generated track session to the list
            trackSessions.Add(trackSession);

            // Move to the next session start time
            startTime = startTime.AddHours(2);
        }
        return trackSessions;
    }
}
