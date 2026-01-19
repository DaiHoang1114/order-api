using Microsoft.EntityFrameworkCore;
using OrderPOC.Application.Orders.Commands;
using OrderPOC.Application.Repositories;
using OrderPOC.Infrastructure.Persistence;
using OrderPOC.Application.Queryable;
using OrderPOC.API.Infrastructure;
using OrderPOC.Infrastructure.Kafka;
using OrderPOC.Application.Kafka;
using OrderPOC.Infrastructure.BackgroundJobs;
using OrderPOC.Application.Orders.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Database
builder.Services.AddDbContext<OrderDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("OrderDb")));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderQueryable, OrderQueryable>();

builder.Services.AddSingleton<IEventProducer, KafkaProducer>();

// Register Email Consumer with Key "Email"
builder.Services.AddKeyedSingleton<IEventConsumer<OrderCreatedIntegrationEvent>>(
    "Email",
    (sp, key) => new KafkaEventConsumer<OrderCreatedIntegrationEvent>(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<ILogger<KafkaEventConsumer<OrderCreatedIntegrationEvent>>>(),
        sp.GetRequiredService<IEventProducer>(),
        groupId: "email-service"));

// Register Inventory Consumer with Key "Inventory"
builder.Services.AddKeyedSingleton<IEventConsumer<OrderCreatedIntegrationEvent>>(
    "Inventory",
    (sp, key) => new KafkaEventConsumer<OrderCreatedIntegrationEvent>(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<ILogger<KafkaEventConsumer<OrderCreatedIntegrationEvent>>>(),
        sp.GetRequiredService<IEventProducer>(),
        groupId: "inventory-service"));
        
builder.Services.AddHostedService<OrderCreatedEmailConsumer>();
builder.Services.AddHostedService<OrderCreatedInventoryConsumer>();

// CQRS
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();
