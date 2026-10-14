namespace EF_TransactionImplementation.Enums
{
    public enum OrderStatus
    {
        Pending = 1,         // Order created but payment not yet confirmed

        PaymentReceived = 2, // Payment captured successfully

        Processing = 3,      // Seller/Warehouse is preparing the order (packing, quality check)

        Shipped = 4,         // Order has left the warehouse and is in transit with the courier

        Delivered = 5,       // Order has been delivered successfully to the customer

        Cancelled = 6        // Order cancelled (by customer, admin, or due to payment/stock issues)
    }
}
