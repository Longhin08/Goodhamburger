using FluentAssertions;
using GoodHamburger.Application.Services;
using Xunit;

namespace GoodHamburger.Tests;

public class DiscountServiceTests
{
    private readonly DiscountService _sut = new();

    [Fact]
    public void Sandwich_SideDish_Drink_Should_Give_20_Percent()
    {
        var result = _sut.GetDiscountPercent(hasSandwich: true, hasSideDish: true, hasDrink: true);
        result.Should().Be(20m);
    }

    [Fact]
    public void Sandwich_Drink_Should_Give_15_Percent()
    {
        var result = _sut.GetDiscountPercent(hasSandwich: true, hasSideDish: false, hasDrink: true);
        result.Should().Be(15m);
    }

    [Fact]
    public void Sandwich_SideDish_Should_Give_10_Percent()
    {
        var result = _sut.GetDiscountPercent(hasSandwich: true, hasSideDish: true, hasDrink: false);
        result.Should().Be(10m);
    }

    [Fact]
    public void Sandwich_Only_Should_Give_No_Discount()
    {
        var result = _sut.GetDiscountPercent(hasSandwich: true, hasSideDish: false, hasDrink: false);
        result.Should().Be(0m);
    }

    [Fact]
    public void No_Sandwich_Should_Give_No_Discount_Even_With_Sides()
    {
        var result = _sut.GetDiscountPercent(hasSandwich: false, hasSideDish: true, hasDrink: true);
        result.Should().Be(0m);
    }
}
