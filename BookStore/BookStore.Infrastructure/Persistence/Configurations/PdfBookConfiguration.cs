using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public class PdfBookConfiguration : IEntityTypeConfiguration<PdfBook>
{
    public void Configure(EntityTypeBuilder<PdfBook> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FilePath).IsRequired().HasMaxLength(255);
        builder.Property(p => p.BookId).IsRequired();
    }
}
