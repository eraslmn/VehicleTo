using APICustomers.Controller;
using APICustomers.Interfaces;
using APICustomers.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace APICustomers.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomer> _mockCustomerService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomerService = new Mock<ICustomer>();
            _controller = new CustomersController(_mockCustomerService.Object);
        }

        [Fact]
        public async Task Post_ValidCustomer_ShouldCallAddCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com"
                // Add other properties as necessary
            };

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            _mockCustomerService.Verify(s => s.AddCustomer(customer), Times.Once);
        }

        [Fact]
        public async Task Post_NullCustomer_ShouldReturnBadRequest()
        {
            // Arrange
            Customer customer = null;

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer cannot be null.", badRequestResult.Value);
            _mockCustomerService.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Post_CustomerWithMissingName_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Email = "john.doe@example.com"
                // Name is missing
            };

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer name and email are required.", badRequestResult.Value);
            _mockCustomerService.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Post_CustomerWithMissingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe"
                // Email is missing
            };

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer name and email are required.", badRequestResult.Value);
            _mockCustomerService.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Post_ServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com"
            };

            _mockCustomerService
                .Setup(s => s.AddCustomer(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error: Test exception", objectResult.Value);
            _mockCustomerService.Verify(s => s.AddCustomer(customer), Times.Once);
        }

        [Fact]
        public async Task Post_CustomerWithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe"
                // Email is missing
            };
            _controller.ModelState.AddModelError("Email", "The Email field is required.");

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _mockCustomerService.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }
    }
}
