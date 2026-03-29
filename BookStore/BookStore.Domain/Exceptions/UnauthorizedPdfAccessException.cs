namespace BookStore.Domain.Exceptions;

public class UnauthorizedPdfAccessException : Exception
{
    public UnauthorizedPdfAccessException(int bookId) : base($"User has not purchased book {bookId} and cannot access its PDF.") { }
}
