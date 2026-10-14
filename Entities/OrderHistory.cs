using EF_TransactionImplementation.Enums;

namespace EF_TransactionImplementation.Entities
{
    public class OrderHistory
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public OrderStatus OldStatus { get; set; }

        public OrderStatus NewStatus { get; set; }

        public string? Remarks { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
