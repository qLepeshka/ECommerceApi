using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrdersApiExample.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        public List<CreateOrderItemDto>? OrderItems { get; set; }
    }
}
