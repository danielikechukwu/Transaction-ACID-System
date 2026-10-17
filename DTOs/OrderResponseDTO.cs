namespace EF_TransactionImplementation.DTOs
{
    public class OrderResponseDTO
    {
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
