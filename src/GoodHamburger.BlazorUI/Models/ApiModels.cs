namespace GoodHamburger.BlazorUI.Models;

public record MenuItem(string Id, string Name, decimal Price, string Type);

public record OrderRequest(string? SandwichId, string? SideDishId, string? DrinkId);

public record OrderResponse(
    Guid Id,
    string? SandwichId, string? SandwichName,
    string? SideDishId, string? SideDishName,
    string? DrinkId,    string? DrinkName,
    decimal Subtotal, decimal DiscountPercent,
    decimal DiscountAmount, decimal Total,
    DateTime CreatedAt, DateTime? UpdatedAt
);
