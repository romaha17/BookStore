namespace BookStore.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string BookName { get; set; } = string.Empty;
    public List<AuthorDto> Authors { get; set; } = new();
    public string Genre { get; set; } = string.Empty;
    public string AvailableLanguage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string SmallDescription { get; set; } = string.Empty;
    public DateTime DateOfPublishment { get; set; }
    public bool HasPdf { get; set; }
}
