using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService) => _orderService = orderService;

    /// <summary>Lists all orders.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>Returns a single order by its ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order is null ? NotFound(new { message = "Pedido não encontrado." }) : Ok(order);
    }

    /// <summary>Creates a new order.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderRequest request)
    {
        var (response, error) = await _orderService.CreateAsync(request);
        if (error is not null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    /// <summary>Updates an existing order.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrderRequest request)
    {
        var (response, error) = await _orderService.UpdateAsync(id, request);
        if (error == "Pedido não encontrado.") return NotFound(new { message = error });
        if (error is not null) return BadRequest(new { message = error });
        return Ok(response);
    }

    /// <summary>Removes an order.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (success, error) = await _orderService.DeleteAsync(id);
        if (!success) return NotFound(new { message = error });
        return NoContent();
    }
}
