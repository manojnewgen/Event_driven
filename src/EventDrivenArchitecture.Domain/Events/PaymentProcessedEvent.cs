namespace EventDrivenArchitecture.Domain.Events;

public record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}
