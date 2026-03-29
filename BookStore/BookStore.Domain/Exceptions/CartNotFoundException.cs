namespace BookStore.Domain.Exceptions;

public class CartNotFoundException : Exception
{
    public CartNotFoundException(string userId) : base($"Cart for user {userId} was not found.") { }
}
