using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Exceptions;
using BookStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookStore.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _carts;
    private readonly IBookRepository _books;
    private readonly IPurchasedBookRepository _purchases;
    private readonly ILogger<CartService> _logger;

    public CartService(ICartRepository carts, IBookRepository books, IPurchasedBookRepository purchases, ILogger<CartService> logger)
    {
        _carts = carts;
        _books = books;
        _purchases = purchases;
        _logger = logger;
    }

    public async Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default)
    {
        var cart = await _carts.GetOrCreateByUserIdAsync(userId, ct);
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                BookId = i.BookId,
                BookName = i.Book?.BookName ?? string.Empty,
                Price = i.Book?.Price ?? 0
            }).ToList()
        };
    }

    public async Task AddItemAsync(string userId, int bookId, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(bookId, ct)
            ?? throw new BookNotFoundException(bookId);

        var cart = await _carts.GetOrCreateByUserIdAsync(userId, ct);

        if (cart.Items.Any(i => i.BookId == bookId))
            throw new BookAlreadyInCartException(bookId);

        cart.Items.Add(new Domain.Entities.CartItem { BookId = bookId });
        await _carts.SaveChangesAsync(ct);
    }

    public async Task RemoveItemAsync(string userId, int cartItemId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct)
            ?? throw new CartNotFoundException(userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is not null)
        {
            await _carts.RemoveItemAsync(item, ct);
            await _carts.SaveChangesAsync(ct);
        }
    }

    public async Task ClearCartAsync(string userId, CancellationToken ct = default)
    {
        await _carts.RemoveAllItemsAsync(userId, ct);
        await _carts.SaveChangesAsync(ct);
    }
}
