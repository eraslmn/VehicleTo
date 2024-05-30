using APIVehicle.Data;
using APIVehicle.Interfaces;
using APIVehicle.Models;
using Microsoft.EntityFrameworkCore;

namespace APIVehicle.Services
{
    public class ServiceVehicle : IVehicle //methods,c
    {
        private readonly DbContextAPI _dbContext; //instance objinc

        public ServiceVehicle(DbContextAPI dbContext) //ctor, di
        {
            _dbContext = dbContext;
        }

        public async Task AddVehicle(Vehicle vehicle) //method addvehicle for db
        {
            if (vehicle == null)
            {
                throw new ArgumentNullException(nameof(vehicle));
            }

            await _dbContext.Vehicles.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteVehicle(int id)
        {
            var vehicle = await _dbContext.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<List<Vehicle>> GetAllVehicles() //retrieve from db
        {
            var vehicles = await _dbContext.Vehicles.ToListAsync();
            return vehicles;
        }

        public async Task<Vehicle> GetVehicleById(int id) //based on its id
        {
            var vehicle = await _dbContext.Vehicles.FindAsync(id);
            return vehicle;
        }

        // Method to update vehicle based from its id
        public async Task UpdateVehicle(int id, Vehicle vehicle) //update
        {
            var vehicleOb = await _dbContext.Vehicles.FindAsync(id);
            vehicleOb.Name = vehicle.Name;
            vehicleOb.ImageUrl = vehicle.ImageUrl;
            vehicleOb.Height = vehicle.Height;
            vehicleOb.Width = vehicle.Width;
            vehicleOb.MaxSpeed = vehicle.MaxSpeed;
            vehicleOb.Price = vehicle.Price;
            vehicleOb.Displacement = vehicle.Displacement;
            await _dbContext.SaveChangesAsync();
        }

    }
}
