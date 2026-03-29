using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;

    public CartRepository(ApplicationDbContext db) => _db = db;

    public Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => _db.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public async Task<Cart> GetOrCreateByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var cart = await GetByUserIdAsync(userId, ct);
        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);
        }
        return cart;
    }

    public async Task RemoveItemAsync(CartItem item, CancellationToken ct = default)
    {
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAllItemsAsync(string userId, CancellationToken ct = default)
    {
        var cart = await GetByUserIdAsync(userId, ct);
        if (cart is not null && cart.Items.Any())
        {
            _db.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
        }
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
