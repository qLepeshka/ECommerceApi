using ECommerceApi.DTOs.Orders;
using ECommerceApi.Models;


namespace ECommerceApi.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> AddItemToOrderAsync(int orderId, AddItemToOrderDto addItemDto);
        Task<bool> RemoveItemFromOrderAsync(int orderId, int productId);
    }
}
