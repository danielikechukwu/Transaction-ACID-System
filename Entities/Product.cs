using Microsoft.EntityFrameworkCore;

namespace EF_TransactionImplementation.Entities
{
    public class Product
    {
        public int Id { get; set; }                      // Primary key

        public string Name { get; set; } = null!;

        public string SKU { get; set; } = null!;         // Unique product code

        public string? Description { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Precision(18, 2)]
        public decimal Discount { get; set; }

        [Precision(18, 2)]
        public decimal FinalPrice { get; set; }          // Price - Discount

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<ProductCategory> ProductCategories { get; set; }
            = new List<ProductCategory>();

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
