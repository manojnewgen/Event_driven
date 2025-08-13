using MassTransit;
using EventDrivenArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EventDrivenArchitecture.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(ILogger<PaymentProcessedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var paymentProcessed = context.Message;
        
        _logger.LogInformation("Processing payment processed event for Order ID: {OrderId}, Payment ID: {PaymentId}, Amount: {Amount}, Status: {Status}", 
            paymentProcessed.OrderId, paymentProcessed.PaymentId, paymentProcessed.Amount, paymentProcessed.Status);

        // Simulate payment processing logic
        await Task.Delay(50);

        if (paymentProcessed.Status == "Success")
        {
            // Here you would typically:
            // 1. Update order status
            // 2. Send confirmation email
            // 3. Update inventory
            // 4. Generate invoice
            
            _logger.LogInformation("Payment successfully processed for Order ID: {OrderId}", paymentProcessed.OrderId);
        }
        else
        {
            _logger.LogWarning("Payment failed for Order ID: {OrderId}, Status: {Status}", 
                paymentProcessed.OrderId, paymentProcessed.Status);
        }
    }
}
