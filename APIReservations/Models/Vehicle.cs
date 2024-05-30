using System.ComponentModel.DataAnnotations;

namespace APIReservations.Models
{
    public class Vehicleeh
    {
        [Key]
        public int VId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
  
}
