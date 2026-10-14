using System.Net;

namespace EF_TransactionImplementation.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerNumber { get; set; } = null!; // Business identifier

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Address> Addresses { get; set; }
            = new List<Address>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}
