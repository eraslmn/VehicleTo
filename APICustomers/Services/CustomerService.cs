using APICustomers.Data;
using APICustomers.Interfaces;
using APICustomers.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace APICustomers.Services
{
    public class CustomerService : ICustomer
    {
        private readonly DbContextAPI _dbContext;
        private readonly ServiceBusSender _serviceBusSender;

        public CustomerService(DbContextAPI dbContext, ServiceBusSender serviceBusSender)
        {
            _dbContext = dbContext;
            _serviceBusSender = serviceBusSender;
        }

        public async Task AddCustomer(Customer customer)
        {
            var vehicleDb = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == customer.VehicleId);

            if (vehicleDb == null && customer.Vehicle != null)
            {
                await _dbContext.Vehicles.AddAsync(customer.Vehicle);
                await _dbContext.SaveChangesAsync();
            }

            // Ensure the Vehicle property is set to null before adding the customer
            customer.Vehicle = null;

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            // Serialize and send the customer object to the Azure Service Bus queue
            string serializedCustomer = JsonConvert.SerializeObject(customer);
            ServiceBusMessage message = new ServiceBusMessage(serializedCustomer);

            try
            {
                await _serviceBusSender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send message to the Azure Service Bus.", ex);
            }
        }
    }
}
