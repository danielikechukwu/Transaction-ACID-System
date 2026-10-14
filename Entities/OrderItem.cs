using Microsoft.EntityFrameworkCore;

namespace EF_TransactionImplementation.Entities
{
    public class OrderItem
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
    }
}
