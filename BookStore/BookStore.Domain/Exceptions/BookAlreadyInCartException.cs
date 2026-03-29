namespace BookStore.Domain.Exceptions;

public class BookAlreadyInCartException : Exception
{
    public BookAlreadyInCartException(int bookId) : base($"Book {bookId} is already in the cart.") { }
}
