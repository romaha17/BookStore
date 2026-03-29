using System.Security.Claims;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Components.Pages;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BookStore.UnitTests;

public class CartPageTests : TestContext
{
    private const string UserId = "test-user";
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly Mock<IStripeCheckoutService> _stripeMock;
    private CartDto _currentCart;

    public CartPageTests()
    {
        _currentCart = new CartDto { Id = 1, UserId = UserId, Items = new() };
        _cartServiceMock = new Mock<ICartService>();
        _stripeMock = new Mock<IStripeCheckoutService>();

        _cartServiceMock
            .Setup(s => s.GetCartAsync(UserId, default))
            .ReturnsAsync(() => _currentCart);

        _cartServiceMock
            .Setup(s => s.RemoveItemAsync(UserId, It.IsAny<int>(), default))
            .Callback<string, int, CancellationToken>((uid, itemId, _) =>
                _currentCart.Items.RemoveAll(i => i.Id == itemId))
            .Returns(Task.CompletedTask);

        _cartServiceMock
            .Setup(s => s.ClearCartAsync(UserId, default))
            .Callback<string, CancellationToken>((_, _) => _currentCart.Items.Clear())
            .Returns(Task.CompletedTask);

        Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(UserId));
        Services.AddScoped<ICartService>(_ => _cartServiceMock.Object);
        Services.AddScoped<IStripeCheckoutService>(_ => _stripeMock.Object);

        var nav = Services.GetRequiredService<FakeNavigationManager>();
        nav.NavigateTo("/cartpage");
    }

    [Fact]
    public async Task ShowsEmptyMessage_WhenCartIsEmpty()
    {
        var cut = RenderComponent<CartPage>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        var alert = cut.Find("div.alert-info");
        Assert.Contains("Your cart is empty", alert.TextContent);
    }

    [Fact]
    public async Task ShowsItemsAndTotal_WhenCartHasItems()
    {
        _currentCart.Items = new()
        {
            new CartItemDto { Id = 1, BookId = 1, BookName = "Alpha", Price = 5m },
            new CartItemDto { Id = 2, BookId = 2, BookName = "Beta", Price = 7m }
        };

        var cut = RenderComponent<CartPage>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        var rows = cut.FindAll("tbody tr");
        Assert.Equal(2, rows.Count);

        Assert.Contains("Alpha", rows[0].TextContent);
        Assert.Contains("$5.00", rows[0].TextContent);
        Assert.Contains("Beta", rows[1].TextContent);
        Assert.Contains("$7.00", rows[1].TextContent);

        var total = cut.Find("h4").TextContent;
        Assert.Contains("$12.00", total);
    }

    [Fact]
    public async Task RemoveItem_RemovesSingleRow()
    {
        _currentCart.Items = new()
        {
            new CartItemDto { Id = 1, BookId = 1, BookName = "One", Price = 3m },
            new CartItemDto { Id = 2, BookId = 2, BookName = "Two", Price = 4m }
        };

        var cut = RenderComponent<CartPage>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        var removeButtons = cut.FindAll("button")
                               .Where(b => b.TextContent.Contains("Remove"))
                               .ToArray();
        removeButtons[0].Click();
        await cut.InvokeAsync(() => Task.CompletedTask);

        var rowsAfter = cut.FindAll("tbody tr");
        Assert.Single(rowsAfter);
        Assert.DoesNotContain("One", rowsAfter[0].TextContent);
        Assert.Contains("Two", rowsAfter[0].TextContent);
    }

    [Fact]
    public async Task ClearCart_RemovesAllRows()
    {
        _currentCart.Items = new()
        {
            new CartItemDto { Id = 1, BookId = 1, BookName = "X", Price = 2m },
            new CartItemDto { Id = 2, BookId = 2, BookName = "Y", Price = 8m }
        };

        var cut = RenderComponent<CartPage>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Find("button.btn-warning").Click();
        await cut.InvokeAsync(() => Task.CompletedTask);

        var alert = cut.Find("div.alert-info");
        Assert.Contains("Your cart is empty", alert.TextContent);
    }

    private class TestAuthStateProvider : AuthenticationStateProvider
    {
        private readonly string _userId;
        public TestAuthStateProvider(string userId) => _userId = userId;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, _userId) },
                authenticationType: "test"
            );
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}