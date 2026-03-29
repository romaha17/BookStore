using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Exceptions;
using BookStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookStore.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchasedBookRepository _purchases;
    private readonly ICartRepository _carts;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(IPurchasedBookRepository purchases, ICartRepository carts, ILogger<PurchaseService> logger)
    {
        _purchases = purchases;
        _carts = carts;
        _logger = logger;
    }

    public async Task<List<BookDto>> GetPurchasedBooksAsync(string userId, CancellationToken ct = default)
    {
        var books = await _purchases.GetBooksByUserIdAsync(userId, ct);
        return books.Select(b => new BookDto
        {
            Id = b.Id,
            BookName = b.BookName,
            Genre = b.Genre,
            AvailableLanguage = b.AvailableLanguage,
            Price = b.Price,
            ImagePath = b.ImagePath,
            SmallDescription = b.SmallDescription,
            DateOfPublishment = b.DateOfPublishment,
            HasPdf = b.PdfBook is not null
        }).ToList();
    }

    public Task<bool> HasPurchasedAsync(string userId, int bookId, CancellationToken ct = default)
        => _purchases.HasUserPurchasedAsync(userId, bookId, ct);

    public async Task AddToLibraryAsync(string userId, int bookId, CancellationToken ct = default)
    {
        if (await _purchases.HasUserPurchasedAsync(userId, bookId, ct))
            throw new BookAlreadyPurchasedException(bookId);

        await _purchases.AddAsync(new PurchasedBook { UserId = userId, BookId = bookId }, ct);
        await _purchases.SaveChangesAsync(ct);
    }

    public async Task CompletePurchaseFromCartAsync(string userId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        if (cart is null || !cart.Items.Any())
        {
            _logger.LogWarning("CompletePurchaseFromCartAsync called for user {UserId} but cart is empty or missing.", userId);
            return;
        }

        var purchases = cart.Items
            .Select(item => new PurchasedBook { UserId = userId, BookId = item.BookId })
            .ToList();

        await _purchases.AddRangeAsync(purchases, ct);
        await _purchases.SaveChangesAsync(ct);

        await _carts.RemoveAllItemsAsync(userId, ct);
        await _carts.SaveChangesAsync(ct);
    }
}
