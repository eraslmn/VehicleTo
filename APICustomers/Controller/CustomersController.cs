using APICustomers.Interfaces;
using APICustomers.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APICustomers.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomer _customerService;

        public CustomersController(ICustomer customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer cannot be null.");
            }

            if (string.IsNullOrEmpty(customer.Name) || string.IsNullOrEmpty(customer.Email))
            {
                return BadRequest("Customer name and email are required.");
            }

            try
            {
                await _customerService.AddCustomer(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}