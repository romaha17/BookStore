using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IStripeCheckoutService
{
    Task<string> CreateCheckoutSessionAsync(CartDto cart, string successUrl, string cancelUrl, CancellationToken ct = default);
}
