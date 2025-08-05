using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ECommerceApi.DTOs.Orders;
using ECommerceApi.Interfaces;
using ECommerceApi.Repositories;

namespace OrdersApiExample.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository; // Для получения цены товара
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderItems = new List<OrderItem>();

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Товар с ID {itemDto.ProductId} не найден.");
                }

                // Добавляем OrderItem с текущей ценой товара
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price // Записываем цену товара на момент создания заказа
                });
            }

            order = await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null) return false;

            // Валидация: статус заказа нельзя изменить на Processing, если он уже был Delivered
            if (existingOrder.Status == OrderStatus.Delivered && updateOrderDto.Status == OrderStatus.Processing)
            {
                throw new InvalidOperationException("Невозможно изменить статус заказа с 'Доставлен' на 'В обработке'.");
            }
            // Можно добавить другие правила перехода статусов

            existingOrder.Status = updateOrderDto.Status;
            // _mapper.Map(updateOrderDto, existingOrder); // Если больше полей для обновления

            return await _orderRepository.UpdateAsync(existingOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> AddItemToOrderAsync(int orderId, AddItemToOrderDto addItemDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            var product = await _productRepository.GetByIdAsync(addItemDto.ProductId);
            if (product == null) throw new ArgumentException($"Товар с ID {addItemDto.ProductId} не найден.");

            var existingOrderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == addItemDto.ProductId);

            if (existingOrderItem != null)
            {
                existingOrderItem.Quantity += addItemDto.Quantity;
                return await _orderRepository.UpdateOrderItemAsync(existingOrderItem);
            }
            else
            {
                var newOrderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = addItemDto.ProductId,
                    Quantity = addItemDto.Quantity,
                    Price = product.Price // Записываем текущую цену товара
                };
                order.OrderItems.Add(newOrderItem); // Добавляем к коллекции заказа
                return await _orderRepository.UpdateAsync(order); // Сохраняем заказ, что обновит OrderItems
            }
        }

        public async Task<bool> RemoveItemFromOrderAsync(int orderId, int productId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            var orderItemToRemove = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (orderItemToRemove == null) return false;

            order.OrderItems.Remove(orderItemToRemove); // Удаляем из коллекции
            return await _orderRepository.UpdateAsync(order); // Сохраняем заказ, что обновит OrderItems
        }
    }
}
