using RacetrackReservations.Models;

namespace RacetrackReservations.Services
{
    public interface TrackSessionService
    {
        void AddTrackSessions(List<TrackSession> trackSessions);
        List<TrackSession> GetAvailableTrackSessions();
        List<TrackSession> GetTrackSessionsForDay(DateTime selectedDate);
        TrackSession GetTrackSessionById(int id);
    }
}
