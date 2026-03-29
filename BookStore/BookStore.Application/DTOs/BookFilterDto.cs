namespace BookStore.Application.DTOs;

public class BookFilterDto
{
    public string? Query { get; set; }
    public string? Genre { get; set; }
    public string? Language { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
}
