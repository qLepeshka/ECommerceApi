using System.ComponentModel.DataAnnotations;

namespace OrdersApiExample.Models.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
