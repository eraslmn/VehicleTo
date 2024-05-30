using APICustomers.Data;
using APICustomers.Models;
using APICustomers.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CustomerServiceTests
{
    private DbContextOptions<DbContextAPI> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DbContextAPI>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
            .Options;
    }

    [Fact]
    public async Task AddCustomer_ShouldAddNewVehicleAndCustomer_WhenVehicleDoesNotExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var vehicle = new Vehiclee { Id = 1, Name = "Vehicle 1" };
        var customer = new Customer { Id = 1, Name = "Customer 1", Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1, Vehicle = vehicle };

        using (var context = new DbContextAPI(options))
        {
            var mockServiceBusSender = new Mock<ServiceBusSender>();
            var customerService = new CustomerService(context, mockServiceBusSender.Object);

            // Act
            await customerService.AddCustomer(customer);

            // Assert
            Assert.Single(context.Vehicles);
            Assert.Single(context.Customers);
            mockServiceBusSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task AddCustomer_ShouldAddCustomerWithoutAddingVehicle_WhenVehicleExists()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var vehicle = new Vehiclee { Id = 1, Name = "Vehicle 1" };
        var customer = new Customer { Id = 1, Name = "Customer 1", Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1 };

        using (var context = new DbContextAPI(options))
        {
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
        }

        using (var context = new DbContextAPI(options))
        {
            var mockServiceBusSender = new Mock<ServiceBusSender>();
            var customerService = new CustomerService(context, mockServiceBusSender.Object);

            // Act
            await customerService.AddCustomer(customer);

            // Assert
            Assert.Single(context.Vehicles);
            Assert.Single(context.Customers);
            mockServiceBusSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task AddCustomer_ShouldThrowException_WhenServiceBusFails()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var vehicle = new Vehiclee { Id = 1, Name = "Vehicle 1" };
        var customer = new Customer { Id = 1, Name = "Customer 1", Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1, Vehicle = vehicle };

        var mockServiceBusSender = new Mock<ServiceBusSender>();
        mockServiceBusSender.Setup(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                             .ThrowsAsync(new Exception("Service Bus failure"));

        using (var context = new DbContextAPI(options))
        {
            var customerService = new CustomerService(context, mockServiceBusSender.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => customerService.AddCustomer(customer));
            Assert.Equal("Failed to send message to the Azure Service Bus.", exception.Message);
        }
    }

    [Fact]
    public async Task AddCustomer_ShouldNotAddCustomer_WhenRequiredFieldIsMissing()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var customer = new Customer { Id = 1, Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1 }; // Missing Name

        using (var context = new DbContextAPI(options))
        {
            var mockServiceBusSender = new Mock<ServiceBusSender>();
            var customerService = new CustomerService(context, mockServiceBusSender.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => customerService.AddCustomer(customer));
            Assert.Empty(context.Customers);
            mockServiceBusSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Fact]
    public async Task AddCustomer_ShouldThrowException_WhenDatabaseSaveFails()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var vehicle = new Vehiclee { Id = 1, Name = "Vehicle 1" };
        var customer = new Customer { Id = 1, Name = "Customer 1", Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1, Vehicle = vehicle };

        var mockServiceBusSender = new Mock<ServiceBusSender>();

        using (var context = new DbContextAPI(options))
        {
            context.Vehicles.Add(vehicle);
            context.Customers.Add(customer);

            var mockDbContext = new Mock<DbContextAPI>(options);
            mockDbContext.Setup(db => db.Vehicles).Returns(context.Vehicles);
            mockDbContext.Setup(db => db.Customers).Returns(context.Customers);
            mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database save failure"));

            var customerService = new CustomerService(mockDbContext.Object, mockServiceBusSender.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => customerService.AddCustomer(customer));
            Assert.Equal("Database save failure", exception.Message);
            mockServiceBusSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Fact]
    public async Task AddCustomer_ShouldAddCustomer_WhenVehicleExistsButNotSetInCustomer()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var vehicle = new Vehiclee { Id = 1, Name = "Vehicle 1" };
        var customer = new Customer { Id = 1, Name = "Customer 1", Email = "customer1@example.com", Phone = "1234567890", VehicleId = 1, Vehicle = null };

        using (var context = new DbContextAPI(options))
        {
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
        }

        using (var context = new DbContextAPI(options))
        {
            var mockServiceBusSender = new Mock<ServiceBusSender>();
            var customerService = new CustomerService(context, mockServiceBusSender.Object);

            // Act
            await customerService.AddCustomer(customer);

            // Assert
            Assert.Single(context.Vehicles);
            Assert.Single(context.Customers);
            mockServiceBusSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}