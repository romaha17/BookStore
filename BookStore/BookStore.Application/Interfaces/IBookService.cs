using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IBookService
{
    Task<PagedResultDto<BookDto>> GetPagedBooksAsync(BookFilterDto filter, CancellationToken ct = default);
    Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct = default);
    Task<List<string>> GetGenresAsync(CancellationToken ct = default);
    Task<List<string>> GetLanguagesAsync(CancellationToken ct = default);
    Task CreateBookAsync(BookDto dto, CancellationToken ct = default);
    Task UpdateBookAsync(BookDto dto, CancellationToken ct = default);
    Task DeleteBookAsync(int id, CancellationToken ct = default);
}
