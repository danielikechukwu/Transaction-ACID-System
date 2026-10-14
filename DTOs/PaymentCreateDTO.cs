using System.ComponentModel.DataAnnotations;

namespace EF_TransactionImplementation.DTOs
{
    public class PaymentCreateDTO
    {
        [Required(ErrorMessage = "Payment Method is required.")]
        [StringLength(50, ErrorMessage = "Payment Method is too long.")]
        public string PaymentMethod { get; set; } = null!;   // e.g., "COD", "Razorpay", "Stripe"

        [StringLength(100, ErrorMessage = "TransactionId is too long.")]
        public string? TransactionId { get; set; }   // Optional (we will generate if not provided)
    }
}
