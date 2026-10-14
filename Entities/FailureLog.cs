namespace EF_TransactionImplementation.Entities
{
    public class FailureLog
    {
        public long Id { get; set; }

        // Optional helper references
        public long? OrderId { get; set; }

        public long? CustomerId { get; set; }

        public long? ProductId { get; set; }

        public long? PaymentId { get; set; }

        // Failure where occurred
        public string MethodName { get; set; } = string.Empty;   // E.g., "PlaceOrder", "ApplyCoupon"

        public string ClassName { get; set; } = string.Empty;

        // Technical details
        public string ErrorMessage { get; set; } = string.Empty;    // Short description of error

        public string StackTrace { get; set; } = string.Empty;      // Full stack trace for debugging

        // Optional data for debugging / investigation
        public string? RequestPayload { get; set; }                 // JSON payload that caused failure (optional)

        public string Severity { get; set; } = "Error";             // Info / Warning / Error / Critical

        public string? CorrelationId { get; set; }                  // For tracing across services

        public string Environment { get; set; } = "Production";     // Or Development / Staging

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    }
}
