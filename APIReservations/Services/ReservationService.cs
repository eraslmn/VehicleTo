using APIReservations.Data;
using APIReservations.Interfaces;
using APIReservations.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace APIReservations.Services
{
    public interface IEmailSender
    {
        void Send(string from, string to, string subject, string body);
    }

    public class SmtpEmailSender : IEmailSender
    {
        public void Send(string from, string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("vehicletoapp@outlook.com", "Vehicleto.123"),
                EnableSsl = true,
            };
            smtpClient.Send(from, to, subject, body);
        }
    }

    public class ReservationService : IReservation
    {
        private readonly DbContextAPI dbContext;
        private readonly IEmailSender emailSender;

        public ReservationService(DbContextAPI context, IEmailSender emailSender)
        {
            dbContext = context;
            this.emailSender = emailSender;
        }

        public async Task<List<Reservations>> GetReservations()
        {
            throw new Exception("Azure Service Bus is unavailable.");
        }

        public async Task UpdateMailStatus(int id)
        {
            var reservationResult = await dbContext.Reservations.FindAsync(id);

            if (reservationResult != null && reservationResult.IsEmailSent == false)
            {
                emailSender.Send("vehicletoapp@outlook.com", reservationResult.Email, "VehicleTo App", "Your test drive is reserved! Email us for more details.");

                reservationResult.IsEmailSent = true;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
