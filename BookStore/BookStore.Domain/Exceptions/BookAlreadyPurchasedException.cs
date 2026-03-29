namespace BookStore.Domain.Exceptions;

public class BookAlreadyPurchasedException : Exception
{
    public BookAlreadyPurchasedException(int bookId) : base($"Book {bookId} has already been purchased.") { }
}
