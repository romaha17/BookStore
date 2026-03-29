namespace BookStore.Domain.Entities;

public class PdfBook
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string FilePath { get; set; } = string.Empty;
}
