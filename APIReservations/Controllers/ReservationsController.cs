using APIReservations.Interfaces;
using APIReservations.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIReservations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservation _reservationService;

        public ReservationsController(IReservation reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<IEnumerable<Reservations>> Get()
        {
            var reservations = await _reservationService.GetReservations();
            return reservations;
        }

        // PUT: api/Reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                await _reservationService.UpdateMailStatus(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}