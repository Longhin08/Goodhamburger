using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly MenuService _menuService;
    private readonly DiscountService _discountService;

    public OrderService(
        IOrderRepository repository,
        MenuService menuService,
        DiscountService discountService)
    {
        _repository = repository;
        _menuService = menuService;
        _discountService = discountService;
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<(OrderResponse? Response, string? Error)> CreateAsync(OrderRequest request)
    {
        var (order, error) = BuildOrder(new Order(), request);
        if (error is not null) return (null, error);

        var created = await _repository.CreateAsync(order!);
        return (MapToResponse(created), null);
    }

    public async Task<(OrderResponse? Response, string? Error)> UpdateAsync(Guid id, OrderRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return (null, "Pedido não encontrado.");

        var (order, error) = BuildOrder(existing, request);
        if (error is not null) return (null, error);

        order!.UpdatedAt = DateTime.UtcNow;
        var updated = await _repository.UpdateAsync(id, order);
        return updated is null ? (null, "Pedido não encontrado.") : (MapToResponse(updated), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? (true, null) : (false, "Pedido não encontrado.");
    }


    private (Order? Order, string? Error) BuildOrder(Order existing, OrderRequest request)
    {

        if (request.SandwichId is null && request.SideDishId is null && request.DrinkId is null)
            return (null, "O pedido deve conter pelo menos um item.");

        var ids = new[] { request.SandwichId, request.SideDishId, request.DrinkId }
                      .Where(x => x is not null).ToList();
        if (ids.Count != ids.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            return (null, "Itens duplicados no pedido. Cada tipo só pode aparecer uma vez.");

        MenuItem? sandwich = null;
        if (request.SandwichId is not null)
        {
            sandwich = _menuService.GetById(request.SandwichId);
            if (sandwich is null)
                return (null, $"Sanduíche '{request.SandwichId}' não encontrado no cardápio.");
            if (sandwich.Type != MenuItemType.Sandwich)
                return (null, $"'{request.SandwichId}' não é um sanduíche.");
        }

        MenuItem? sideDish = null;
        if (request.SideDishId is not null)
        {
            sideDish = _menuService.GetById(request.SideDishId);
            if (sideDish is null)
                return (null, $"Acompanhamento '{request.SideDishId}' não encontrado no cardápio.");
            if (sideDish.Type != MenuItemType.SideDish)
                return (null, $"'{request.SideDishId}' não é um acompanhamento.");
        }

        MenuItem? drink = null;
        if (request.DrinkId is not null)
        {
            drink = _menuService.GetById(request.DrinkId);
            if (drink is null)
                return (null, $"Bebida '{request.DrinkId}' não encontrada no cardápio.");
            if (drink.Type != MenuItemType.Drink)
                return (null, $"'{request.DrinkId}' não é uma bebida.");
        }

        decimal subtotal = (sandwich?.Price ?? 0) + (sideDish?.Price ?? 0) + (drink?.Price ?? 0);
        decimal discountPct = _discountService.GetDiscountPercent(
            sandwich is not null, sideDish is not null, drink is not null);
        decimal discountAmt = Math.Round(subtotal * discountPct / 100, 2);
        decimal total = subtotal - discountAmt;

        existing.SandwichId = sandwich?.Id;
        existing.SideDishId = sideDish?.Id;
        existing.DrinkId = drink?.Id;
        existing.Subtotal = subtotal;
        existing.DiscountPercent = discountPct;
        existing.DiscountAmount = discountAmt;
        existing.Total = total;

        return (existing, null);
    }

    private OrderResponse MapToResponse(Order order)
    {
        var sandwich = order.SandwichId is not null ? _menuService.GetById(order.SandwichId) : null;
        var sideDish  = order.SideDishId  is not null ? _menuService.GetById(order.SideDishId)  : null;
        var drink     = order.DrinkId     is not null ? _menuService.GetById(order.DrinkId)     : null;

        return new OrderResponse(
            order.Id,
            sandwich?.Id, sandwich?.Name,
            sideDish?.Id,  sideDish?.Name,
            drink?.Id,     drink?.Name,
            order.Subtotal,
            order.DiscountPercent,
            order.DiscountAmount,
            order.Total,
            order.CreatedAt,
            order.UpdatedAt
        );
    }
}
