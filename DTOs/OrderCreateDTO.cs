using System.ComponentModel.DataAnnotations;

namespace EF_TransactionImplementation.DTOs
{
    public class OrderCreateDTO
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "ShippingAddressId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ShippingAddressId must be a positive value.")]
        public int ShippingAddressId { get; set; }

        [Required(ErrorMessage = "BillingAddressId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BillingAddressId must be a positive value.")]
        public int BillingAddressId { get; set; }

        [Required(ErrorMessage = "At least one order item is required.")]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<OrderItemCreateDTO> Items { get; set; } = new();

        [Required(ErrorMessage = "Payment details are required.")]
        public PaymentCreateDTO Payment { get; set; } = null!;

        // Use this only for testing implicit transaction behavior
        public bool MakeSecondSaveFail { get; set; } = false;

    }
}
