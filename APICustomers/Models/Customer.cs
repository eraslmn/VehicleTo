﻿namespace APICustomers.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int VehicleId { get; set; }

        public Vehiclee Vehicle { get; set; }
    }
}
