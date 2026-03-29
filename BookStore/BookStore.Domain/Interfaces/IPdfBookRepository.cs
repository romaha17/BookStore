using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IPdfBookRepository
{
    Task<PdfBook?> GetByBookIdAsync(int bookId, CancellationToken ct = default);
}
