using Microsoft.AspNetCore.Mvc;
using EventDrivenArchitecture.Publishers.Services;
using EventDrivenArchitecture.Domain.Events;

namespace EventDrivenArchitecture.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentPublisher _paymentPublisher;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentPublisher paymentPublisher, ILogger<PaymentController> logger)
    {
        _paymentPublisher = paymentPublisher;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        try
        {
            // Simulate payment processing with 90% success rate
            var isSuccessful = Random.Shared.NextDouble() > 0.1;
            
            var paymentEvent = new PaymentProcessedEvent
            {
                OrderId = request.OrderId,
                PaymentId = Guid.NewGuid(),
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Status = isSuccessful ? "Success" : "Failed",
                ProcessedAt = DateTime.UtcNow
            };

            await _paymentPublisher.PublishPaymentProcessedAsync(paymentEvent);
            
            _logger.LogInformation("Payment processed and event published: Order {OrderId}, Payment {PaymentId}, Status: {Status}", 
                paymentEvent.OrderId, paymentEvent.PaymentId, paymentEvent.Status);
            
            return Ok(new { 
                PaymentId = paymentEvent.PaymentId, 
                OrderId = paymentEvent.OrderId,
                Status = paymentEvent.Status,
                Message = $"Payment {paymentEvent.Status.ToLower()}" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return StatusCode(500, "Internal server error");
        }
    }
}

public record ProcessPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod
);
