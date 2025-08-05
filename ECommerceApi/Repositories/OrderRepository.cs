using ECommerceApi.Data;
using ECommerceApi.Interfaces;
using ECommerceApi.Models;

using Microsoft.EntityFrameworkCore;


namespace ECommerceApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemAsync(int orderId, int productId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.ProductId == productId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}