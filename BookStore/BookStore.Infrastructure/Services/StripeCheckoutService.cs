using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BookStore.Infrastructure.Services;

public class StripeCheckoutService : IStripeCheckoutService
{
    public StripeCheckoutService(IOptions<StripeSettings> options)
    {
        StripeConfiguration.ApiKey = options.Value.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(CartDto cart, string successUrl, string cancelUrl, CancellationToken ct = default)
    {
        var lineItems = cart.Items.Select(item => new SessionLineItemOptions
        {
            Quantity = 1,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmountDecimal = (long)(item.Price * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.BookName
                }
            }
        }).ToList();

        var options = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
        return session.Url;
    }
}
