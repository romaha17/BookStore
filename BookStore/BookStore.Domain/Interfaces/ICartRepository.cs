using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Cart> GetOrCreateByUserIdAsync(string userId, CancellationToken ct = default);
    Task RemoveItemAsync(CartItem item, CancellationToken ct = default);
    Task RemoveAllItemsAsync(string userId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
