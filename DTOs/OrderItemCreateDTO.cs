using System.ComponentModel.DataAnnotations;

namespace EF_TransactionImplementation.DTOs
{
    public class OrderItemCreateDTO
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 1000, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }
    }
}
