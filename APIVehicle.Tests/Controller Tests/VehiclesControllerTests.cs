using Xunit;
using Moq;
using APIVehicle.Controllers;
using APIVehicle.Interfaces;
using APIVehicle.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleTestProject.Controllers
{
    public class VehiclesControllerTests
    {
        private readonly VehiclesController _controller;
        private readonly Mock<IVehicle> _mockVehicleService;

        public VehiclesControllerTests()
        {
            _mockVehicleService = new Mock<IVehicle>();
            _controller = new VehiclesController(_mockVehicleService.Object);
        }

        [Fact]
        public async Task Get_ReturnsListOfVehicles()
        {
            var sampleVehicles = GetSampleVehicles();
            _mockVehicleService.Setup(service => service.GetAllVehicles()).ReturnsAsync(sampleVehicles);

            var result = await _controller.Get();

            var vehicles = Assert.IsType<List<Vehicle>>(result);
            Assert.Equal(sampleVehicles.Count, vehicles.Count);
        }

        [Fact]
        public async Task Get_ReturnsEmptyList()
        {
            _mockVehicleService.Setup(service => service.GetAllVehicles()).ReturnsAsync(new List<Vehicle>());

            var result = await _controller.Get();

            var vehicles = Assert.IsType<List<Vehicle>>(result);
            Assert.Empty(vehicles);
        }

        [Fact]
        public async Task Get_ById_ReturnsVehicle()
        {
            var sampleVehicle = GetSampleVehicle();
            _mockVehicleService.Setup(service => service.GetVehicleById(1)).ReturnsAsync(sampleVehicle);

            var result = await _controller.Get(1);

            Assert.IsType<Vehicle>(result);
            Assert.Equal(sampleVehicle.Name, result.Name);
        }

        [Fact]
        public async Task Get_ById_ReturnsNull()
        {
            _mockVehicleService.Setup(service => service.GetVehicleById(1)).ReturnsAsync((Vehicle)null);

            var result = await _controller.Get(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task Post_CreatesVehicle()
        {
            var newVehicle = GetSampleVehicle();

            await _controller.Post(newVehicle);

            _mockVehicleService.Verify(service => service.AddVehicle(newVehicle), Times.Once);
        }

        [Fact]
        public async Task Put_UpdatesVehicle()
        {
            var updatedVehicle = GetSampleVehicle();

            await _controller.Put(1, updatedVehicle);

            _mockVehicleService.Verify(service => service.UpdateVehicle(1, updatedVehicle), Times.Once);
        }

        [Fact]
        public async Task Put_UpdatesNonExistentVehicle()
        {
            _mockVehicleService.Setup(service => service.UpdateVehicle(It.IsAny<int>(), It.IsAny<Vehicle>())).ThrowsAsync(new KeyNotFoundException());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Put(1, GetSampleVehicle()));
        }

        [Fact]
        public async Task Delete_RemovesVehicle()
        {
            var vehicleId = 1;

            await _controller.Delete(vehicleId);

            _mockVehicleService.Verify(service => service.DeleteVehicle(vehicleId), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistentVehicle_DoesNotRemove()
        {
            var vehicleId = 1;
            _mockVehicleService.Setup(service => service.DeleteVehicle(vehicleId)).ThrowsAsync(new KeyNotFoundException());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Delete(vehicleId));
        }

        [Fact]
        public async Task Get_ReturnsCorrectVehicleById()
        {
            var sampleVehicle = GetSampleVehicle();
            _mockVehicleService.Setup(service => service.GetVehicleById(sampleVehicle.Id)).ReturnsAsync(sampleVehicle);

            var result = await _controller.Get(sampleVehicle.Id);

            Assert.Equal(sampleVehicle.Id, result.Id);
        }

        [Fact]
        public async Task Post_CallsAddVehicleOnce()
        {
            var newVehicle = GetSampleVehicle();

            await _controller.Post(newVehicle);

            _mockVehicleService.Verify(service => service.AddVehicle(newVehicle), Times.Once);
        }

        [Fact]
        public async Task Put_CallsUpdateVehicleOnce()
        {
            var updatedVehicle = GetSampleVehicle();

            await _controller.Put(updatedVehicle.Id, updatedVehicle);

            _mockVehicleService.Verify(service => service.UpdateVehicle(updatedVehicle.Id, updatedVehicle), Times.Once);
        }

        [Fact]
        public async Task Delete_CallsDeleteVehicleOnce()
        {
            var vehicleId = 1;

            await _controller.Delete(vehicleId);

            _mockVehicleService.Verify(service => service.DeleteVehicle(vehicleId), Times.Once);
        }

        [Fact]
        public async Task GetAllVehicles_ReturnsCorrectCount()
        {
            var sampleVehicles = GetSampleVehicles();
            _mockVehicleService.Setup(service => service.GetAllVehicles()).ReturnsAsync(sampleVehicles);

            var result = await _controller.Get();

            var vehicles = Assert.IsType<List<Vehicle>>(result);
            Assert.Equal(sampleVehicles.Count, vehicles.Count);
        }

        [Fact]
        public async Task GetVehicleById_ReturnsCorrectVehicle()
        {
            var sampleVehicle = GetSampleVehicle();
            _mockVehicleService.Setup(service => service.GetVehicleById(sampleVehicle.Id)).ReturnsAsync(sampleVehicle);

            var result = await _controller.Get(sampleVehicle.Id);

            Assert.IsType<Vehicle>(result);
            Assert.Equal(sampleVehicle.Name, result.Name);
        }

        private static List<Vehicle> GetSampleVehicles()
        {
            return new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Toyota Corolla", Price = 20000, ImageUrl = "https://example.com/corolla.jpg", Displacement = "1800cc", MaxSpeed = "180km/h", Length = 4.5, Width = 1.8, Height = 1.5 },
                new Vehicle { Id = 2, Name = "Honda Civic", Price = 22000, ImageUrl = "https://example.com/civic.jpg", Displacement = "2000cc", MaxSpeed = "200km/h", Length = 4.6, Width = 1.9, Height = 1.5 },
                new Vehicle { Id = 3, Name = "Ford Focus", Price = 21000, ImageUrl = "https://example.com/focus.jpg", Displacement = "1600cc", MaxSpeed = "190km/h", Length = 4.4, Width = 1.8, Height = 1.4 }
            };
        }

        private static Vehicle GetSampleVehicle()
        {
            return new Vehicle { Id = 1, Name = "Toyota Corolla", Price = 20000, ImageUrl = "https://example.com/corolla.jpg", Displacement = "1800cc", MaxSpeed = "180km/h", Length = 4.5, Width = 1.8, Height = 1.5 };
        }
    }
}