using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.BookName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Genre).IsRequired().HasMaxLength(100);
        builder.Property(b => b.AvailableLanguage).HasMaxLength(100);
        builder.Property(b => b.ImagePath).HasMaxLength(500);
        builder.Property(b => b.SmallDescription).HasMaxLength(2000);
        builder.Property(b => b.Price).HasColumnType("decimal(18,2)");
        builder.Property(b => b.DateOfPublishment).HasColumnType("timestamp with time zone");
        builder.HasMany(b => b.Authors).WithMany(a => a.Books);
        builder.HasOne(b => b.PdfBook).WithOne().HasForeignKey<PdfBook>(p => p.BookId);
    }
}
