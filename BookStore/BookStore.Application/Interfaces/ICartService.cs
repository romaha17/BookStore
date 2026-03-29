using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default);
    Task AddItemAsync(string userId, int bookId, CancellationToken ct = default);
    Task RemoveItemAsync(string userId, int cartItemId, CancellationToken ct = default);
    Task ClearCartAsync(string userId, CancellationToken ct = default);
}
