using APIReservations.Models;
using Microsoft.EntityFrameworkCore;

namespace APIReservations.Data
{
    public class DbContextAPI : DbContext
    {
        public DbSet<Vehicleeh> Vehicles { get; set; }
        public DbSet<Reservations> Reservations { get; set; }

        public DbContextAPI(DbContextOptions<DbContextAPI> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure here if options are not provided from outside
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ReservationApiDb;");
            }
        }
    }
}
