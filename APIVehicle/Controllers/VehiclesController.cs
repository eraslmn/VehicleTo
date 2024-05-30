using APIVehicle.Interfaces;
using APIVehicle.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIVehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {


        private IVehicle _serviceVehicle;

        public VehiclesController (IVehicle serviceVehicle)
        {
            _serviceVehicle = serviceVehicle;
        }


        // GET: api/<VehiclesController>
        [HttpGet]
        public async Task<IEnumerable<Vehicle>> Get()
        {
          var vehicles = await  _serviceVehicle.GetAllVehicles(); //return list of vehicles
          return vehicles;
          
        }

        // GET api/<VehiclesController>/5
        [HttpGet("{id}")]
        public async Task<Vehicle> Get(int id)
        {
            return await _serviceVehicle.GetVehicleById(id);
        }

        // POST api/<VehiclesController>
        [HttpPost]
        public async Task Post([FromBody] Vehicle vehicle)
        {
            await _serviceVehicle.AddVehicle(vehicle);
        }

        // PUT api/<VehiclesController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Vehicle vehicle)
        {
            await _serviceVehicle.UpdateVehicle(id, vehicle);

        }

        // DELETE api/<VehiclesController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _serviceVehicle.DeleteVehicle(id);
        }
    }
}
