using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _db;

    public AuthorRepository(ApplicationDbContext db) => _db = db;

    public async Task<Author> FindOrCreateAsync(string firstName, string lastName, CancellationToken ct = default)
    {
        var author = await _db.Authors
            .SingleOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName, ct);

        if (author is null)
        {
            author = new Author { FirstName = firstName, LastName = lastName };
            _db.Authors.Add(author);
        }

        return author;
    }
}
