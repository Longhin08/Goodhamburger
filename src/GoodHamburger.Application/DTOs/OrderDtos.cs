namespace GoodHamburger.Application.DTOs;

/// <summary>Request body for creating or updating an order.</summary>
public record OrderRequest(
    string? SandwichId,
    string? SideDishId,
    string? DrinkId
);

/// <summary>Response returned to the client.</summary>
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
