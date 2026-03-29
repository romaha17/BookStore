using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class PdfBookRepository : IPdfBookRepository
{
    private readonly ApplicationDbContext _db;

    public PdfBookRepository(ApplicationDbContext db) => _db = db;

    public Task<PdfBook?> GetByBookIdAsync(int bookId, CancellationToken ct = default)
        => _db.PdfBooks.FirstOrDefaultAsync(pb => pb.BookId == bookId, ct);
}
