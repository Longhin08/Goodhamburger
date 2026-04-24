using System.Collections.Concurrent;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.API.Infrastructure;

/// <summary>Thread-safe in-memory implementation of IOrderRepository.</summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values.OrderBy(o => o.CreatedAt));

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var order) ? order : null);

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
        Task.FromResult(_store.TryRemove(id, out _));
}
