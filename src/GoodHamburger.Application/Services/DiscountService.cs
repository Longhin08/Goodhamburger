using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Services;

public class DiscountService
{
    /// <summary>
    /// Returns the discount percentage (0, 10, 15, or 20) based on order composition.
    /// Rules:
    ///   Sandwich + SideDish + Drink  => 20%
    ///   Sandwich + Drink             => 15%
    ///   Sandwich + SideDish          => 10%
    ///   Sandwich only                =>  0%
    /// </summary>
    public decimal GetDiscountPercent(bool hasSandwich, bool hasSideDish, bool hasDrink)
    {
        if (!hasSandwich) return 0m;

        return (hasSideDish, hasDrink) switch
        {
            (true, true)   => 20m,
            (false, true)  => 15m,
            (true, false)  => 10m,
            _              => 0m
        };
    }
}
