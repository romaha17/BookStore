using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public class PurchasedBookConfiguration : IEntityTypeConfiguration<PurchasedBook>
{
    public void Configure(EntityTypeBuilder<PurchasedBook> builder)
    {
        builder.HasKey(pb => pb.Id);
        builder.Property(pb => pb.UserId).IsRequired();
        builder.Property(pb => pb.BookId).IsRequired();
        builder.HasIndex(pb => new { pb.UserId, pb.BookId }).IsUnique();
        builder.HasOne(pb => pb.Book).WithMany().HasForeignKey(pb => pb.BookId);
    }
}
