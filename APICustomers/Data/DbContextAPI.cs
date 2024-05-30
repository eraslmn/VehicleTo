using APICustomers.Models;
using Microsoft.EntityFrameworkCore;

namespace APICustomers.Data
{
    public class DbContextAPI : DbContext
    {
        public DbContextAPI(DbContextOptions<DbContextAPI> options) : base(options)
        {
        }

        public virtual DbSet<Vehiclee> Vehicles { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CustomerApiDb;");
            }
        }
    }
}
