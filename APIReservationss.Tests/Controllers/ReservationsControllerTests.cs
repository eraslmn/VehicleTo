using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using APIReservations.Controllers;
using APIReservations.Interfaces;
using APIReservations.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ReservationsControllerTests
{
    private readonly Mock<IReservation> _mockReservationService;
    private readonly ReservationsController _controller;

    public ReservationsControllerTests()
    {
        _mockReservationService = new Mock<IReservation>();
        _controller = new ReservationsController(_mockReservationService.Object);
    }

    [Fact]
    public async Task TestGetReservations_ReturnsExpectedReservations()
    {
        // Arrange
        var reservations = new List<Reservations>
        {
            new Reservations { Id = 1, Name = "John Doe" }
        };
        _mockReservationService.Setup(service => service.GetReservations()).ReturnsAsync(reservations);

        // Act
        var result = await _controller.Get();

        // Assert
        var actionResult = Assert.IsAssignableFrom<IEnumerable<Reservations>>(result);
        Assert.Single(actionResult);
    }

    [Fact]
    public async Task TestGetReservations_ReturnsEmptyList()
    {
        // Arrange
        _mockReservationService.Setup(service => service.GetReservations()).ReturnsAsync(new List<Reservations>());

        // Act
        var result = await _controller.Get();

        // Assert
        var actionResult = Assert.IsAssignableFrom<IEnumerable<Reservations>>(result);
        Assert.Empty(actionResult);
    }

    [Fact]
    public async Task TestGetReservations_ThrowsException()
    {
        // Arrange
        _mockReservationService.Setup(service => service.GetReservations()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Get());
    }

    [Theory]
    [InlineData(1)]
    public async Task TestPut_UpdateMailStatus_ValidId(int id)
    {
        // Act
        var result = await _controller.Put(id);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockReservationService.Verify(service => service.UpdateMailStatus(id), Times.Once);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task TestPut_InvalidId_DoesNotUpdateMailStatus(int id)
    {
        // Act
        var result = await _controller.Put(id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _mockReservationService.Verify(service => service.UpdateMailStatus(id), Times.Never);
    }

    [Fact]
    public async Task TestPut_UpdateMailStatus_ServiceThrowsException()
    {
        // Arrange
        var id = 1;
        _mockReservationService.Setup(service => service.UpdateMailStatus(id)).ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await _controller.Put(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
        _mockReservationService.Verify(service => service.UpdateMailStatus(id), Times.Once);
    }

    [Theory]
    [InlineData(2)]
    public async Task TestPut_UpdateMailStatus_AlreadySent(int id)
    {
        // Arrange
        var reservation = new Reservations { Id = id, IsEmailSent = true };
        _mockReservationService.Setup(service => service.UpdateMailStatus(id)).Callback(() => reservation.IsEmailSent = true);

        // Act
        var result = await _controller.Put(id);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.True(reservation.IsEmailSent);
        _mockReservationService.Verify(service => service.UpdateMailStatus(id), Times.Once);
    }
}