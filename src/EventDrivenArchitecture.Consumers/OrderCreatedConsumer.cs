using MassTransit;
using EventDrivenArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EventDrivenArchitecture.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderCreated = context.Message;
        
        _logger.LogInformation("Processing order created event for Order ID: {OrderId}, Customer: {CustomerId}, Amount: {Amount}", 
            orderCreated.OrderId, orderCreated.CustomerId, orderCreated.TotalAmount);

        // Simulate order processing logic
        await Task.Delay(100);

        // Here you would typically:
        // 1. Validate the order
        // 2. Check inventory
        // 3. Reserve products
        // 4. Calculate pricing
        // 5. Publish OrderProcessedEvent

        _logger.LogInformation("Successfully processed order created event for Order ID: {OrderId}", orderCreated.OrderId);
    }
}
