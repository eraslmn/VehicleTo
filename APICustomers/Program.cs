

using APICustomers.Data;
using APICustomers.Interfaces;
using APICustomers.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomer, CustomerService>();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
string queueName = builder.Configuration["ServiceBus:QueueName"];

builder.Services.AddDbContext<DbContextAPI>(options =>
    options.UseSqlServer(connectionString));

// Register ServiceBusClient
builder.Services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(serviceBusConnectionString));

// Register ServiceBusSender
builder.Services.AddSingleton<ServiceBusSender>(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(queueName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();