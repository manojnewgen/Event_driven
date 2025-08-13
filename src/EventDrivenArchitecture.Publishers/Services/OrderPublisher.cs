using MassTransit;
using EventDrivenArchitecture.Domain.Events;

namespace EventDrivenArchitecture.Publishers.Services;

public interface IOrderPublisher
{
    Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken = default);
    Task PublishOrderProcessedAsync(OrderProcessedEvent orderProcessedEvent, CancellationToken cancellationToken = default);
}

public class OrderPublisher : IOrderPublisher
{
    // is this object composition
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(orderCreatedEvent, cancellationToken);
    }

    public async Task PublishOrderProcessedAsync(OrderProcessedEvent orderProcessedEvent, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(orderProcessedEvent, cancellationToken);
    }
}
