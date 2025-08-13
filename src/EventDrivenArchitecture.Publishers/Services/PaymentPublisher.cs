using MassTransit;
using EventDrivenArchitecture.Domain.Events;

namespace EventDrivenArchitecture.Publishers.Services;

public interface IPaymentPublisher
{
    Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken cancellationToken = default);
}

public class PaymentPublisher : IPaymentPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken cancellationToken = default)
    {
        await _publishEndpoint.Publish(paymentProcessedEvent, cancellationToken);
    }
}
