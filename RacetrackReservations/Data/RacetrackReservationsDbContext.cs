using RacetrackReservations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RacetrackReservations.Data
{
    public class RacetrackReservationsDbContext : IdentityDbContext<User>
    {
        public RacetrackReservationsDbContext(DbContextOptions<RacetrackReservationsDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<TrackSession> TrackSessions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
