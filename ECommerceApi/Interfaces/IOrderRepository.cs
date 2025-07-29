using ECommerceApi.Models;
using System.Threading.Tasks;

namespace ECommerceApi.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> AddOrderAsync(Order order);

        Task<Order> GetOrderByIdAsync(int id);

        Task UpdateOrderAsync(Order order);

        Task DeleteOrderAsync(int id);


    }
}

