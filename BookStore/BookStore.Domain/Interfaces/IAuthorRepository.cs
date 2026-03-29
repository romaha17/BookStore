using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<Author> FindOrCreateAsync(string firstName, string lastName, CancellationToken ct = default);
}
