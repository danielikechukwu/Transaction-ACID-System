namespace EF_TransactionImplementation.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,      // Payment initiated

        Paid = 2,         // Payment completed

        Failed = 3,       // Payment failed due to any reason

        Refunded = 4      // Refund processed
    }
}
