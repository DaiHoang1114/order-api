using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderPOC.Application.Orders.Commands;

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


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand request)
    {
        var orderId = await mediator.Send(
            new CreateOrderCommand(request.CustomerId));

        return CreatedAtAction(nameof(Create), new { id = orderId }, orderId);
    }
    
}