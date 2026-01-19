using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderPOC.Application.Orders.Commands;
using OrderPOC.Application.Orders.Queries;

namespace OrderPOC.API.Orders;

[ApiController]
[Route("api/v1/orders")]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Order API is working!");
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(Guid orderId)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(orderId));

        return Ok(order);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand request)
    {
        var orderId = await mediator.Send(
            new CreateOrderCommand(request.CustomerId));

        return CreatedAtAction(nameof(Create), new { id = orderId }, orderId);
    }
}