using FluentAssertions;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using Xunit;

namespace GoodHamburger.Tests;

public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values);

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var o) ? o : null);

    public Task<Order> CreateAsync(Order order)
    {
        _store[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<Order?> UpdateAsync(Guid id, Order order)
    {
        if (!_store.ContainsKey(id)) return Task.FromResult<Order?>(null);
        _store[id] = order;
        return Task.FromResult<Order?>(order);
    }

    public Task<bool> DeleteAsync(Guid id) =>
        Task.FromResult(_store.Remove(id));
}

public class OrderServiceTests
{
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(
            new FakeOrderRepository(),
            new MenuService(),
            new DiscountService());
    }

    [Fact]
    public async Task Create_Full_Order_Should_Apply_20_Percent_Discount()
    {
        var req = new OrderRequest("x-burger", "fries", "soda");
        var (response, error) = await _sut.CreateAsync(req);

        error.Should().BeNull();
        response!.DiscountPercent.Should().Be(20m);
        response.Subtotal.Should().Be(9.50m);          // 5.00 + 2.00 + 2.50
        response.DiscountAmount.Should().Be(1.90m);    // 9.50 * 20%
        response.Total.Should().Be(7.60m);
    }

    [Fact]
    public async Task Create_Sandwich_And_Drink_Should_Apply_15_Percent()
    {
        var req = new OrderRequest("x-egg", null, "soda");
        var (response, error) = await _sut.CreateAsync(req);

        error.Should().BeNull();
        response!.DiscountPercent.Should().Be(15m);
        response.Subtotal.Should().Be(7.00m);          // 4.50 + 2.50
        response.DiscountAmount.Should().Be(1.05m);
        response.Total.Should().Be(5.95m);
    }

    [Fact]
    public async Task Create_Sandwich_And_Fries_Should_Apply_10_Percent()
    {
        var req = new OrderRequest("x-bacon", "fries", null);
        var (response, error) = await _sut.CreateAsync(req);

        error.Should().BeNull();
        response!.DiscountPercent.Should().Be(10m);
        response.Subtotal.Should().Be(9.00m);          // 7.00 + 2.00
        response.DiscountAmount.Should().Be(0.90m);
        response.Total.Should().Be(8.10m);
    }

    [Fact]
    public async Task Create_Sandwich_Only_Should_Have_No_Discount()
    {
        var req = new OrderRequest("x-burger", null, null);
        var (response, error) = await _sut.CreateAsync(req);

        error.Should().BeNull();
        response!.DiscountPercent.Should().Be(0m);
        response.Total.Should().Be(5.00m);
    }

    [Fact]
    public async Task Create_With_Empty_Order_Should_Return_Error()
    {
        var req = new OrderRequest(null, null, null);
        var (response, error) = await _sut.CreateAsync(req);

        response.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Create_With_Invalid_Item_Id_Should_Return_Error()
    {
        var req = new OrderRequest("nao-existe", null, null);
        var (response, error) = await _sut.CreateAsync(req);

        response.Should().BeNull();
        error.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Create_With_Wrong_Item_Type_In_Field_Should_Return_Error()
    {
        var req = new OrderRequest("soda", null, null);
        var (response, error) = await _sut.CreateAsync(req);

        response.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Delete_Nonexistent_Order_Should_Return_Error()
    {
        var (success, error) = await _sut.DeleteAsync(Guid.NewGuid());

        success.Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetById_After_Create_Should_Return_Order()
    {
        var req = new OrderRequest("x-burger", "fries", "soda");
        var (created, _) = await _sut.CreateAsync(req);

        var found = await _sut.GetByIdAsync(created!.Id);

        found.Should().NotBeNull();
        found!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Update_Should_Recalculate_Pricing()
    {
        var (created, _) = await _sut.CreateAsync(new OrderRequest("x-burger", "fries", "soda"));

        var (updated, error) = await _sut.UpdateAsync(created!.Id, new OrderRequest("x-bacon", null, null));

        error.Should().BeNull();
        updated!.DiscountPercent.Should().Be(0m);
        updated.Total.Should().Be(7.00m);
    }
}
