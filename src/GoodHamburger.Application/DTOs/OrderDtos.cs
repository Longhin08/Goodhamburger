namespace GoodHamburger.Application.DTOs;

public record OrderRequest(
    string? SandwichId,
    string? SideDishId,
    string? DrinkId
);

public record OrderResponse(
    Guid Id,
    string? SandwichId,
    string? SandwichName,
    string? SideDishId,
    string? SideDishName,
    string? DrinkId,
    string? DrinkName,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
