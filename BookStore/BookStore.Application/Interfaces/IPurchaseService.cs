using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IPurchaseService
{
    Task<List<BookDto>> GetPurchasedBooksAsync(string userId, CancellationToken ct = default);
    Task<bool> HasPurchasedAsync(string userId, int bookId, CancellationToken ct = default);
    Task AddToLibraryAsync(string userId, int bookId, CancellationToken ct = default);
    Task CompletePurchaseFromCartAsync(string userId, CancellationToken ct = default);
}
