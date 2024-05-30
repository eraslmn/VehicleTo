using APIVehicle.Models;
using Microsoft.EntityFrameworkCore;

namespace APIVehicle.Data
{
    public class DbContextAPI : DbContext
    {
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public DbContextAPI(DbContextOptions<DbContextAPI> options) : base(options)
        {
        }

        // Optionally keep this method for fallback purposes
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=VehicleApiDb;");
            }
        }
    }
}