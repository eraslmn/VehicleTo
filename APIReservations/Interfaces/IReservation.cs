using APIReservations.Models;

namespace APIReservations.Interfaces
{
    public interface IReservation
    {
        Task<List<Reservations>> GetReservations(); //from azure service bus queue, once we look at the details well send the mail
        Task UpdateMailStatus(int id);
    }
}
