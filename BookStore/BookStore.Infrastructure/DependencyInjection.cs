using Amazon.S3;
using BookStore.Application.Interfaces;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPdfBookRepository, PdfBookRepository>();
        services.AddScoped<IPurchasedBookRepository, PurchasedBookRepository>();

        services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();
        services.AddScoped<IStorageService, S3StorageService>();
        services.AddScoped<IRecommendationService, OpenAiRecommendationService>();

        return services;
    }
}
