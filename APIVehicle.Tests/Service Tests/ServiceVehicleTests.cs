using APIVehicle.Data;
using APIVehicle.Models;
using APIVehicle.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace APIVehicle.Tests.Service_Tests
{
    public class ServiceVehicleTests
    {
        private readonly DbContextAPI _dbContext;
        private readonly ServiceVehicle _service;

        public ServiceVehicleTests()
        {
            var options = new DbContextOptionsBuilder<DbContextAPI>()
                .UseInMemoryDatabase(databaseName: "VehicleApiDb")
                .Options;

            _dbContext = new DbContextAPI(options);

            // Ensure database is clean before seeding
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Seed the in-memory database with some data if necessary
            SeedDatabase();

            _service = new ServiceVehicle(_dbContext);
        }

        private void SeedDatabase()
        {
            // Ensure no duplicates and clear existing data
            _dbContext.Vehicles.RemoveRange(_dbContext.Vehicles);

            // Seed initial data for testing
            _dbContext.Vehicles.AddRange(
                new Vehicle { Id = 1, Name = "Toyota Corolla", Price = 20000, ImageUrl = "https://example.com/corolla.jpg", Displacement = "1800cc", MaxSpeed = "180km/h", Length = 4.5, Width = 1.8, Height = 1.5 },
                new Vehicle { Id = 2, Name = "Honda Civic", Price = 22000, ImageUrl = "https://example.com/civic.jpg", Displacement = "2000cc", MaxSpeed = "200km/h", Length = 4.6, Width = 1.9, Height = 1.5 }
            );
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddVehicle_ShouldAddVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 3, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };

            // Act
            await _service.AddVehicle(vehicle);

            // Assert
            var addedVehicle = await _dbContext.Vehicles.FindAsync(vehicle.Id);
            Assert.NotNull(addedVehicle);
            Assert.Equal(vehicle.Name, addedVehicle.Name);
        }

        [Fact]
        public async Task AddVehicle_NullVehicle_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddVehicle(null));
        }

        [Fact]
        public async Task AddVehicle_AddAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 4, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };

            var mockSet = new Mock<DbSet<Vehicle>>();
            mockSet.Setup(m => m.AddAsync(It.IsAny<Vehicle>(), default))
                   .ThrowsAsync(new DbUpdateException());

            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles).Returns(mockSet.Object);

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => service.AddVehicle(vehicle));
        }


        [Fact]
        public async Task AddVehicle_SaveChangesAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 4, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };
            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles.AddAsync(vehicle, default))
                       .ThrowsAsync(new DbUpdateException());

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => service.AddVehicle(vehicle));
        }

        [Fact]
        public async Task AddVehicle_ShouldCallSaveChangesAsyncAfterAddAsync()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 3, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };

            // Act
            await _service.AddVehicle(vehicle);

            // Assert
            var addedVehicle = await _dbContext.Vehicles.FindAsync(vehicle.Id);
            Assert.NotNull(addedVehicle);
            Assert.Equal(vehicle.Name, addedVehicle.Name);
        }

        [Fact]
        public async Task DeleteVehicle_ShouldDeleteVehicle()
        {
            // Arrange
            var vehicle = await _dbContext.Vehicles.FindAsync(1);

            // Act
            await _service.DeleteVehicle(1);

            // Assert
            var deletedVehicle = await _dbContext.Vehicles.FindAsync(1);
            Assert.Null(deletedVehicle);
        }

        [Fact]
        public async Task DeleteVehicle_FindAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            _dbContext.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.DeleteVehicle(1));
        }

        [Fact]
        public async Task DeleteVehicle_RemoveThrowsException_ShouldPropagateException()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };

            var mockSet = new Mock<DbSet<Vehicle>>();
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(new List<Vehicle> { vehicle }.AsQueryable().Provider);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(new List<Vehicle> { vehicle }.AsQueryable().Expression);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(new List<Vehicle> { vehicle }.AsQueryable().ElementType);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(new List<Vehicle> { vehicle }.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(vehicle);
            mockSet.Setup(m => m.Remove(It.IsAny<Vehicle>())).Throws(new InvalidOperationException());

            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles).Returns(mockSet.Object);

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteVehicle(1));
        }


        [Fact]
        public async Task DeleteVehicle_SaveChangesAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Name = "Car", Displacement = "1600cc", ImageUrl = "https://example.com/car.jpg", MaxSpeed = "150km/h" };

            var mockSet = new Mock<DbSet<Vehicle>>();
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(new List<Vehicle> { vehicle }.AsQueryable().Provider);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(new List<Vehicle> { vehicle }.AsQueryable().Expression);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(new List<Vehicle> { vehicle }.AsQueryable().ElementType);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(new List<Vehicle> { vehicle }.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(vehicle);

            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(default))
                       .ThrowsAsync(new DbUpdateConcurrencyException());

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.DeleteVehicle(1));
        }


        [Fact]
        public async Task GetVehicleById_ShouldReturnVehicle()
        {
            // Arrange
            var expectedVehicle = await _dbContext.Vehicles.FindAsync(1);

            // Act
            var result = await _service.GetVehicleById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVehicle.Name, result.Name);
        }

        [Fact]
        public async Task GetVehicleById_NonExistentVehicle_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetVehicleById(100);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVehicleById_FindAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Vehicle>>();
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                   .ThrowsAsync(new InvalidOperationException());

            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles).Returns(mockSet.Object);

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetVehicleById(1));
        }

        [Fact]
        public async Task UpdateVehicle_ShouldUpdateVehicle()
        {
            // Arrange
            var existingVehicle = await _dbContext.Vehicles.FindAsync(1);
            var updatedVehicle = new Vehicle { Name = "NewCar", ImageUrl = "NewUrl", Height = 5, Width = 2, MaxSpeed = "200", Price = 30000, Displacement = "2.0" };

            // Act
            await _service.UpdateVehicle(1, updatedVehicle);

            // Assert
            var vehicle = await _dbContext.Vehicles.FindAsync(1);
            Assert.Equal("NewCar", vehicle.Name);
        }

        [Fact]
        public async Task UpdateVehicle_FindAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var updatedVehicle = new Vehicle { Name = "NewCar", ImageUrl = "NewUrl", Height = 5, Width = 2, MaxSpeed = "200", Price = 30000, Displacement = "2.0" };
            _dbContext.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _service.UpdateVehicle(1, updatedVehicle));
        }

        [Fact]
        public async Task UpdateVehicle_SaveChangesAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var existingVehicle = new Vehicle { Id = 1, Name = "OldCar" };
            var updatedVehicle = new Vehicle { Name = "NewCar", ImageUrl = "NewUrl", Height = 5, Width = 2, MaxSpeed = "200", Price = 30000, Displacement = "2.0" };

            var mockSet = new Mock<DbSet<Vehicle>>();
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(new List<Vehicle> { existingVehicle }.AsQueryable().Provider);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(new List<Vehicle> { existingVehicle }.AsQueryable().Expression);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(new List<Vehicle> { existingVehicle }.AsQueryable().ElementType);
            mockSet.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(new List<Vehicle> { existingVehicle }.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(existingVehicle);

            var mockContext = new Mock<DbContextAPI>(new DbContextOptions<DbContextAPI>());
            mockContext.Setup(m => m.Vehicles).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(default))
                       .ThrowsAsync(new DbUpdateConcurrencyException());

            var service = new ServiceVehicle(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.UpdateVehicle(1, updatedVehicle));
        }

    }
}