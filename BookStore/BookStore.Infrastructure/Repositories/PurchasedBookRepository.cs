using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class PurchasedBookRepository : IPurchasedBookRepository
{
    private readonly ApplicationDbContext _db;

    public PurchasedBookRepository(ApplicationDbContext db) => _db = db;

    public Task<List<Book>> GetBooksByUserIdAsync(string userId, CancellationToken ct = default)
        => _db.PurchasedBooks
            .Where(pb => pb.UserId == userId)
            .Select(pb => pb.Book)
            .Include(b => b.PdfBook)
            .ToListAsync(ct);

    public Task<bool> HasUserPurchasedAsync(string userId, int bookId, CancellationToken ct = default)
        => _db.PurchasedBooks.AnyAsync(pb => pb.UserId == userId && pb.BookId == bookId, ct);

    public async Task AddAsync(PurchasedBook purchase, CancellationToken ct = default)
    {
        _db.PurchasedBooks.Add(purchase);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<PurchasedBook> purchases, CancellationToken ct = default)
    {
        await _db.PurchasedBooks.AddRangeAsync(purchases, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
