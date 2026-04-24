namespace GoodHamburger.Domain.Entities;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string? SandwichId { get; set; }
    public string? SideDishId { get; set; }
    public string? DrinkId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
}
