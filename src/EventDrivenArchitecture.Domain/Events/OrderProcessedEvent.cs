namespace EventDrivenArchitecture.Domain.Events;

public record OrderProcessedEvent
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
    public string ProcessedBy { get; init; } = string.Empty;
}
