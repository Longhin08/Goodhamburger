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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order is null ? NotFound(new { message = "Pedido não encontrado." }) : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderRequest request)
    {
        var (response, error) = await _orderService.CreateAsync(request);
        if (error is not null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrderRequest request)
    {
        var (response, error) = await _orderService.UpdateAsync(id, request);
        if (error == "Pedido não encontrado.") return NotFound(new { message = error });
        if (error is not null) return BadRequest(new { message = error });
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (success, error) = await _orderService.DeleteAsync(id);
        if (!success) return NotFound(new { message = error });
        return NoContent();
    }
}
