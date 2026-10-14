using EF_TransactionImplementation.Enums;
using Microsoft.EntityFrameworkCore;

namespace EF_TransactionImplementation.Entities
{
    public class Order
    {
        public long Id { get; set; }

        public string OrderNumber { get; set; } = null!;

        public int CustomerId { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string ShippingAddress { get; set; } = null!;

        public string BillingAddress { get; set; } = null!;

        // Navigation
        public Customer Customer { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public Payment? Payment { get; set; }

        public ICollection<OrderHistory> History { get; set; }
            = new List<OrderHistory>();
    }
}
