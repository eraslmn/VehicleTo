using APIReservations.Data;
using APIReservations.Models;
using APIReservations.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

public class ServiceReservationsTests
{
    private DbContextAPI GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DbContextAPI>()
                      .UseInMemoryDatabase(databaseName: dbName)
                      .Options;
        return new DbContextAPI(options);
    }

    [Fact]
    public async Task GetReservations_ThrowsException()
    {
        // Arrange
        var context = GetDbContext("GetReservationsTestDb");
        context.Database.EnsureDeleted();
        var emailSenderMock = new Moq.Mock<APIReservations.Services.IEmailSender>();
        var reservationService = new ReservationService(context, emailSenderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => reservationService.GetReservations());
        Assert.Equal("Azure Service Bus is unavailable.", exception.Message);
    }

    [Fact]
    public async Task UpdateMailStatus_UpdatesEmailStatus()
    {
        // Arrange
        var context = GetDbContext("UpdateMailStatusTestDb");
        context.Database.EnsureDeleted();
        var reservation = new Reservations { Id = 1, Name = "Test", Email = "test@test.com", Phone = "1234567890", IsEmailSent = false };
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var emailSenderMock = new Moq.Mock<APIReservations.Services.IEmailSender>();
        var reservationService = new ReservationService(context, emailSenderMock.Object);

        // Act
        await reservationService.UpdateMailStatus(1);

        // Assert
        emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        var updatedReservation = context.Reservations.Find(1);
        Assert.True(updatedReservation.IsEmailSent);
    }

    [Fact]
    public async Task UpdateMailStatus_InvalidId_DoesNothing()
    {
        // Arrange
        var context = GetDbContext("InvalidIdTestDb");
        context.Database.EnsureDeleted();
        var emailSenderMock = new Moq.Mock<APIReservations.Services.IEmailSender>();
        var reservationService = new ReservationService(context, emailSenderMock.Object);

        // Act
        await reservationService.UpdateMailStatus(999); // Invalid ID

        // Assert
        emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        var reservation = await context.Reservations.FindAsync(999);
        Assert.Null(reservation); // No changes made
    }

    [Fact]
    public async Task UpdateMailStatus_EmailAlreadySent_DoesNothing()
    {
        // Arrange
        var context = GetDbContext("EmailAlreadySentTestDb");
        context.Database.EnsureDeleted();
        var reservation = new Reservations { Id = 1, Name = "Test", Email = "test@test.com", Phone = "1234567890", IsEmailSent = true };
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var emailSenderMock = new Moq.Mock<APIReservations.Services.IEmailSender>();
        var reservationService = new ReservationService(context, emailSenderMock.Object);

        // Act
        await reservationService.UpdateMailStatus(1);

        // Assert
        emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        var updatedReservation = context.Reservations.Find(1);
        Assert.True(updatedReservation.IsEmailSent);
    }

    [Fact]
    public async Task UpdateMailStatus_EmailSendingFails_ThrowsException()
    {
        // Arrange
        var context = GetDbContext("EmailSendingFailsTestDb");
        context.Database.EnsureDeleted();
        var reservation = new Reservations { Id = 1, Name = "Test", Email = "test@test.com", Phone = "1234567890", IsEmailSent = false };
        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();

        var emailSenderMock = new Moq.Mock<APIReservations.Services.IEmailSender>();
        emailSenderMock.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Throws(new SmtpException("Failed to send email"));
        var reservationService = new ReservationService(context, emailSenderMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<SmtpException>(() => reservationService.UpdateMailStatus(1));
    }
}
