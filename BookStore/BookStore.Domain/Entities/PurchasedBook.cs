namespace BookStore.Domain.Entities;

public class PurchasedBook
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}
