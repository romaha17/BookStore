using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(List<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<List<Book>> SearchAsync(string? query, string? genre, string? language, decimal? maxPrice, CancellationToken ct = default);
    Task<List<string>> GetDistinctGenresAsync(CancellationToken ct = default);
    Task<List<string>> GetDistinctLanguagesAsync(CancellationToken ct = default);
    Task AddAsync(Book book, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
