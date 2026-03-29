using Xunit;
using System.Linq;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Persistence;

namespace BookStore.UnitTests;

public class ApplicationDbContextTests
{
    [Fact]
    public void CanAddAndRetrieveABook()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var book = new Book { BookName = "My Test Book", Price = 9.99M };

        // Act
        context.Books.Add(book);
        context.SaveChanges();

        // Assert
        var loaded = context.Books.FirstOrDefault(b => b.BookName == "My Test Book");
        Assert.NotNull(loaded);
        Assert.Equal(9.99M, loaded.Price);
    }
}