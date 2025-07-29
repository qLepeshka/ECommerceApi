using ECommerceApi.Interfaces;
using ECommerceApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{id}", Name = "GetOrderById")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order newOrder)
        {

            if (newOrder == null)
            {
                return BadRequest("Order data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newOrder.Items != null && newOrder.Items.Any())
            {
                newOrder.TotalAmount = newOrder.Items.Sum(item => item.Quantity * item.Price);
            }
            else
            {
                newOrder.TotalAmount = 0;
            }

            if (newOrder.OrderDate == default(System.DateTime))
            {
                newOrder.OrderDate = System.DateTime.UtcNow;
            }


            var createdOrder = await _orderRepository.AddOrderAsync(newOrder);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }
    }
}
