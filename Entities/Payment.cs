using EF_TransactionImplementation.Enums;
using Microsoft.EntityFrameworkCore;

namespace EF_TransactionImplementation.Entities
{
    public class Payment
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public Order Order { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string PaymentMethod { get; set; } = null!;

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
