using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IPurchasedBookRepository
{
    Task<List<Book>> GetBooksByUserIdAsync(string userId, CancellationToken ct = default);
    Task<bool> HasUserPurchasedAsync(string userId, int bookId, CancellationToken ct = default);
    Task AddAsync(PurchasedBook purchase, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<PurchasedBook> purchases, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
