using MassTransit;
using EventDrivenArchitecture.Publishers.Services;
using EventDrivenArchitecture.Consumers;
using EventDrivenArchitecture.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MassTransit with RabbitMQ (only when available)
var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:Host");
var connectionString = builder.Configuration.GetConnectionString("ServiceBus");

// Only configure MassTransit if we have a messaging provider available
if (!string.IsNullOrEmpty(rabbitHost) || !string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddMassTransit(x =>
    {
        // Add consumers
        x.AddConsumer<OrderCreatedConsumer>();
        x.AddConsumer<PaymentProcessedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            var host = rabbitHost ?? "localhost";
            var rabbitUsername = builder.Configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
            var rabbitPassword = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";

            cfg.Host(host, "/", h =>
            {
                h.Username(rabbitUsername);
                h.Password(rabbitPassword);
            });

            cfg.ConfigureEndpoints(context);
        });
    });
    
    // Register publishers only if MassTransit is configured
    builder.Services.AddScoped<IOrderPublisher, OrderPublisher>();
    builder.Services.AddScoped<IPaymentPublisher, PaymentPublisher>();
}
else
{
    // Register mock publishers when no messaging is available
    builder.Services.AddScoped<IOrderPublisher, MockOrderPublisher>();
    builder.Services.AddScoped<IPaymentPublisher, MockPaymentPublisher>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
