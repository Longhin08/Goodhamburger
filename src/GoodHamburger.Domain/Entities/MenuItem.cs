namespace GoodHamburger.Domain.Entities;

public enum MenuItemType
{
    Sandwich,
    SideDish,
    Drink
}

public class MenuItem
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public MenuItemType Type { get; init; }
}
