using OrdersApiExample.Models.Domain;
using OrdersApiExample.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersApiExample.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
