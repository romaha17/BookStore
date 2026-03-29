using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _db;

    public BookRepository(ApplicationDbContext db) => _db = db;

    public Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Books.Include(b => b.Authors).Include(b => b.PdfBook).FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<(List<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var total = await _db.Books.CountAsync(ct);
        var items = await _db.Books
            .Include(b => b.Authors)
            .Include(b => b.PdfBook)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public async Task<List<Book>> SearchAsync(string? query, string? genre, string? language, decimal? maxPrice, CancellationToken ct = default)
    {
        var q = _db.Books.Include(b => b.Authors).Include(b => b.PdfBook).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(b => b.BookName.ToLower().Contains(query.ToLower()));

        if (!string.IsNullOrWhiteSpace(genre))
            q = q.Where(b => b.Genre == genre);

        if (!string.IsNullOrWhiteSpace(language))
            q = q.Where(b => b.AvailableLanguage == language);

        if (maxPrice.HasValue)
            q = q.Where(b => b.Price <= maxPrice);

        return await q.ToListAsync(ct);
    }

    public Task<List<string>> GetDistinctGenresAsync(CancellationToken ct = default)
        => _db.Books.Select(b => b.Genre).Distinct().ToListAsync(ct);

    public Task<List<string>> GetDistinctLanguagesAsync(CancellationToken ct = default)
        => _db.Books.Select(b => b.AvailableLanguage).Distinct().ToListAsync(ct);

    public async Task AddAsync(Book book, CancellationToken ct = default)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Book book, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _db.Books.Include(b => b.PdfBook).FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book is not null)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync(ct);
        }
    }
}
