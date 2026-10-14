using EF_TransactionImplementation.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF_TransactionImplementation.Data
{
    public class EF_TransactionDbContext : DbContext
    {

        public EF_TransactionDbContext(DbContextOptions<EF_TransactionDbContext> options)
            : base(options)
        {
        }

        // ----------------- DbSet Properties -----------------
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<OrderHistory> OrderHistories { get; set; } = null!;
        public DbSet<FailureLog> FailureLogs => Set<FailureLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and constraints if needed

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>() // Store enum as string
                .HasMaxLength(30); // Optional: Set max length for the string representation of the enum

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<OrderHistory>()
                .Property(h => h.OldStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<OrderHistory>()
                .Property(h => h.NewStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            ConfigureProductCategoryMapping(modelBuilder);
            SeedInitialData(modelBuilder);

        }

        private void ConfigureProductCategoryMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });   

                entity.HasOne(pc => pc.Product)
                      .WithMany(p => p.ProductCategories)
                      .HasForeignKey(pc => pc.ProductId);   
                
                entity.HasOne(pc => pc.Category)
                      .WithMany(c => c.ProductCategories)
                      .HasForeignKey(pc => pc.CategoryId);              
            });
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // 1. Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic gadgets and devices", IsActive = true },
                new Category { Id = 2, Name = "Books", Description = "Books and study materials", IsActive = true },
                new Category { Id = 3, Name = "Clothing", Description = "Men and Women apparel", IsActive = true }
            );

            // 2. Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Samsung Smartphone",
                    SKU = "SM-001",
                    Description = "Latest model smartphone",
                    Price = 20000,
                    Discount = 2000,
                    FinalPrice = 18000,
                    StockQuantity = 50,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01)
                },
                new Product
                {
                    Id = 2,
                    Name = "Dell Laptop",
                    SKU = "LP-101",
                    Description = "High performance laptop",
                    Price = 60000,
                    Discount = 5000,
                    FinalPrice = 55000,
                    StockQuantity = 20,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01)
                },
                new Product
                {
                    Id = 3,
                    Name = "Python Programming Book",
                    SKU = "BK-777",
                    Description = "Beginner to Advanced Python",
                    Price = 800,
                    Discount = 100,
                    FinalPrice = 700,
                    StockQuantity = 100,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01)
                }
            );

            // 3. Seed ProductCategory (many-to-many)
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { ProductId = 1, CategoryId = 1, LinkedAt = new DateTime(2024, 01, 01) },
                new ProductCategory { ProductId = 2, CategoryId = 1, LinkedAt = new DateTime(2024, 01, 01) },
                new ProductCategory { ProductId = 3, CategoryId = 2, LinkedAt = new DateTime(2024, 01, 01) }
            );

            // 4. Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    CustomerNumber = "CUST-0001",
                    FirstName = "Pranaya",
                    LastName = "Rout",
                    Email = "pranaya@example.com",
                    Phone = "9999999999",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01)
                },
                new Customer
                {
                    Id = 2,
                    CustomerNumber = "CUST-0002",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Phone = "8888888888",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 01, 01)
                }
            );

            // 5. Seed Addresses
            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    Id = 1,
                    CustomerId = 1,
                    Line1 = "123 Main Street",
                    City = "Bhubaneswar",
                    State = "Odisha",
                    Country = "India",
                    ZipCode = "751024",
                    IsDefaultBilling = true,
                    IsDefaultShipping = true
                },
                new Address
                {
                    Id = 2,
                    CustomerId = 2,
                    Line1 = "MG Road",
                    City = "Bangalore",
                    State = "Karnataka",
                    Country = "India",
                    ZipCode = "560001",
                    IsDefaultBilling = true,
                    IsDefaultShipping = true
                }
            );
        }
    }
}
