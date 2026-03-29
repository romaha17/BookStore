namespace BookStore.Application.DTOs;

public class CartItemDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
