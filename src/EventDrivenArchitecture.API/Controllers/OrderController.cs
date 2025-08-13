using Microsoft.AspNetCore.Mvc;
using EventDrivenArchitecture.Publishers.Services;
using EventDrivenArchitecture.Domain.Events;

namespace EventDrivenArchitecture.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderPublisher _orderPublisher;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderPublisher orderPublisher, ILogger<OrderController> logger)
    {
        _orderPublisher = orderPublisher;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _orderPublisher.PublishOrderCreatedAsync(orderEvent);
            
            _logger.LogInformation("Order created and event published: {OrderId}", orderEvent.OrderId);
            
            return Ok(new { OrderId = orderEvent.OrderId, Message = "Order created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{orderId}/process")]
    public async Task<IActionResult> ProcessOrder(Guid orderId)
    {
        try
        {
            var orderProcessedEvent = new OrderProcessedEvent
            {
                OrderId = orderId,
                Status = "Processed",
                ProcessedAt = DateTime.UtcNow,
                ProcessedBy = "System"
            };

            await _orderPublisher.PublishOrderProcessedAsync(orderProcessedEvent);
            
            _logger.LogInformation("Order processed event published: {OrderId}", orderId);
            
            return Ok(new { OrderId = orderId, Status = "Processed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
            return StatusCode(500, "Internal server error");
        }
    }
}

public record CreateOrderRequest(
    string CustomerId,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
    string ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
