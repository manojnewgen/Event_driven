using EventDrivenArchitecture.Domain.Events;
using EventDrivenArchitecture.Publishers.Services;

namespace EventDrivenArchitecture.API.Services
{
    /// <summary>
    /// Mock implementation of IOrderPublisher for when messaging is not available
    /// </summary>
    public class MockOrderPublisher : IOrderPublisher
    {
        private readonly ILogger<MockOrderPublisher> _logger;

        public MockOrderPublisher(ILogger<MockOrderPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("MOCK: Order created event - OrderId: {OrderId}, CustomerId: {CustomerId}, Amount: {Amount}", 
                orderEvent.OrderId, orderEvent.CustomerId, orderEvent.TotalAmount);
            
            // Simulate async operation
            await Task.Delay(10, cancellationToken);
        }

        public async Task PublishOrderProcessedAsync(OrderProcessedEvent orderEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("MOCK: Order processed event - OrderId: {OrderId}, Status: {Status}", 
                orderEvent.OrderId, orderEvent.Status);
            
            // Simulate async operation
            await Task.Delay(10, cancellationToken);
        }
    }

    /// <summary>
    /// Mock implementation of IPaymentPublisher for when messaging is not available
    /// </summary>
    public class MockPaymentPublisher : IPaymentPublisher
    {
        private readonly ILogger<MockPaymentPublisher> _logger;

        public MockPaymentPublisher(ILogger<MockPaymentPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("MOCK: Payment processed event - PaymentId: {PaymentId}, OrderId: {OrderId}, Amount: {Amount}, Method: {Method}", 
                paymentEvent.PaymentId, paymentEvent.OrderId, paymentEvent.Amount, paymentEvent.PaymentMethod);
            
            // Simulate async operation
            await Task.Delay(10, cancellationToken);
        }
    }
}
