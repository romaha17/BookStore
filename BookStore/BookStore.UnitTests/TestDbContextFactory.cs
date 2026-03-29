using System;
using Microsoft.EntityFrameworkCore;
using BookStore.Infrastructure.Persistence;

namespace BookStore.UnitTests;

public static class TestDbContextFactory
{
    // Creates a new ApplicationDbContext backed by a fresh in-memory database.
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}