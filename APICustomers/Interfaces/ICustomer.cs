using APICustomers.Models;

namespace APICustomers.Interfaces
{
    public interface ICustomer
    {
        Task AddCustomer(Customer customer);
    }
}
