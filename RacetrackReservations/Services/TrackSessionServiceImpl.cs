using RacetrackReservations.Data;
using RacetrackReservations.Models;

namespace RacetrackReservations.Services
{
    public class TrackSessionServiceImpl : TrackSessionService
    {
        private readonly RacetrackReservationsDbContext _racetrackReservationsDbContext;

        public TrackSessionServiceImpl(RacetrackReservationsDbContext racetrackReservationsDbContext)
        {
            _racetrackReservationsDbContext = racetrackReservationsDbContext;
        }


        // Adds a list of track sessions to the database
        public void AddTrackSessions(List<TrackSession> trackSessions)
        {
            _racetrackReservationsDbContext.TrackSessions.AddRange(trackSessions);
            _racetrackReservationsDbContext.SaveChanges();
        }


        // Retrieves all available track sessions from the database
        public List<TrackSession> GetAvailableTrackSessions()
        {
            return _racetrackReservationsDbContext.TrackSessions.ToList();
        }


        // Retrieves track sessions for a specific day from the database
        public List<TrackSession> GetTrackSessionsForDay(DateTime selectedDate)
        {
            return _racetrackReservationsDbContext.TrackSessions
                .Where(session => session.StartTime.Date == selectedDate.Date)
                .ToList();
        }


        // Retrieves details of a specific track session by its ID from the database
        public TrackSession GetTrackSessionById(int id)
        {
            return _racetrackReservationsDbContext.TrackSessions.FirstOrDefault(session => session.TrackSessionId == id);
        }
    }
}
