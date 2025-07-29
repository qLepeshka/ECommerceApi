using OrdersApiExample.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerceApi.Models;

namespace OrdersApiExample.Services
{
    public class OrderService : IOrderService
    {
        private static List<Order> _orders = new List<Order>();
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
            new Product { Id = 2, Name = "Mouse", Price = 25.00m },
            new Product { Id = 3, Name = "Keyboard", Price = 75.00m },
            new Product { Id = 4, Name = "Monitor", Price = 300.00m }
        };
        private static int _nextOrderId = 1;
        private static int _nextOrderItemId = 1;

        public OrderService()
        {

            if (!_orders.Any())
            {
                var initialOrderDto = new CreateOrderDto
                {
                    CustomerName = "John Doe",
                    Address = "123 Main St",
                    OrderItems = new List<CreateOrderItemDto>
                    {
                        new CreateOrderItemDto { ProductId = 1, Quantity = 1 },
                        new CreateOrderItemDto { ProductId = 2, Quantity = 2 }
                    }
                };
                CreateOrderAsync(initialOrderDto).Wait();
            }
        }

        public Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orderDtos = _orders.Select(MapToOrderDto).ToList();
            return Task.FromResult<IEnumerable<OrderDto>>(orderDtos);
        }

        public Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order == null ? null : MapToOrderDto(order));
        }

        public Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var newOrder = new Order
            {
                Id = Interlocked.Increment(ref _nextOrderId),
                CustomerName = createOrderDto.CustomerName,
                Address = createOrderDto.Address,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;
            if (createOrderDto.OrderItems != null)
            {
                foreach (var itemDto in createOrderDto.OrderItems)
                {
                    var product = _products.FirstOrDefault(p => p.Id == itemDto.ProductId);
                    if (product == null)
                    {
                        return Task.FromResult<OrderDto?>(null);
                    }

                    var orderItem = new OrderItem
                    {
                        Id = Interlocked.Increment(ref _nextOrderItemId),
                        OrderId = newOrder.Id,
                        ProductId = itemDto.ProductId,
                        ProductName = product.Name,
                        Quantity = itemDto.Quantity,
                        PriceAtOrder = product.Price
                    };
                    newOrder.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Quantity * orderItem.PriceAtOrder;
                }
            }
            newOrder.TotalAmount = totalAmount;

            _orders.Add(newOrder);
            return Task.FromResult(MapToOrderDto(newOrder));
        }

        public Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder == null)
            {
                return Task.FromResult(false);
            }

            existingOrder.CustomerName = updateOrderDto.CustomerName;
            existingOrder.Address = updateOrderDto.Address;
            existingOrder.Status = updateOrderDto.Status;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteOrderAsync(int id)
        {
            var orderToRemove = _orders.FirstOrDefault(o => o.Id == id);
            if (orderToRemove == null)
            {
                return Task.FromResult(false);
            }

            _orders.Remove(orderToRemove);
            return Task.FromResult(true);
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Address = order.Address,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(MapToOrderItemDto).ToList()
            };
        }

        private OrderItemDto MapToOrderItemDto(OrderItem orderItem)
        {
            return new OrderItemDto
            {
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                Quantity = orderItem.Quantity,
                PriceAtOrder = orderItem.PriceAtOrder
            };
        }
    }
}
