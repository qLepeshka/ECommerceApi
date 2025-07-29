using System.ComponentModel.DataAnnotations;

namespace OrdersApiExample.Models.DTOs
{
    public class UpdateOrderDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
