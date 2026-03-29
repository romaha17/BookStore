using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Exceptions;
using BookStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookStore.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _books;
    private readonly IAuthorRepository _authors;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository books, IAuthorRepository authors, ILogger<BookService> logger)
    {
        _books = books;
        _authors = authors;
        _logger = logger;
    }

    public async Task<PagedResultDto<BookDto>> GetPagedBooksAsync(BookFilterDto filter, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(filter.Query) || !string.IsNullOrWhiteSpace(filter.Genre) ||
            !string.IsNullOrWhiteSpace(filter.Language) || filter.MaxPrice.HasValue)
        {
            var searchResults = await _books.SearchAsync(filter.Query, filter.Genre, filter.Language, filter.MaxPrice, ct);
            var totalSearch = searchResults.Count;
            var pagedSearch = searchResults
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResultDto<BookDto>
            {
                Items = pagedSearch.Select(ToDto).ToList(),
                TotalCount = totalSearch,
                TotalPages = (int)Math.Ceiling((double)totalSearch / filter.PageSize),
                CurrentPage = filter.Page
            };
        }

        var (items, totalCount) = await _books.GetPagedAsync(filter.Page, filter.PageSize, ct);
        return new PagedResultDto<BookDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
            CurrentPage = filter.Page
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        return book is null ? null : ToDto(book);
    }

    public Task<List<string>> GetGenresAsync(CancellationToken ct = default)
        => _books.GetDistinctGenresAsync(ct);

    public Task<List<string>> GetLanguagesAsync(CancellationToken ct = default)
        => _books.GetDistinctLanguagesAsync(ct);

    public async Task CreateBookAsync(BookDto dto, CancellationToken ct = default)
    {
        var book = new Book
        {
            BookName = dto.BookName,
            Genre = dto.Genre,
            AvailableLanguage = dto.AvailableLanguage,
            Price = dto.Price,
            ImagePath = dto.ImagePath,
            SmallDescription = dto.SmallDescription,
            DateOfPublishment = dto.DateOfPublishment == default ? DateTime.UtcNow : dto.DateOfPublishment
        };

        foreach (var authorDto in dto.Authors)
        {
            var author = await _authors.FindOrCreateAsync(authorDto.FirstName, authorDto.LastName, ct);
            book.Authors.Add(author);
        }

        await _books.AddAsync(book, ct);
    }

    public async Task UpdateBookAsync(BookDto dto, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(dto.Id, ct)
            ?? throw new BookNotFoundException(dto.Id);

        book.BookName = dto.BookName;
        book.Genre = dto.Genre;
        book.AvailableLanguage = dto.AvailableLanguage;
        book.Price = dto.Price;
        book.SmallDescription = dto.SmallDescription;
        if (!string.IsNullOrEmpty(dto.ImagePath))
            book.ImagePath = dto.ImagePath;

        book.Authors.Clear();
        foreach (var authorDto in dto.Authors)
        {
            var author = await _authors.FindOrCreateAsync(authorDto.FirstName, authorDto.LastName, ct);
            book.Authors.Add(author);
        }

        await _books.UpdateAsync(book, ct);
    }

    public async Task DeleteBookAsync(int id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct)
            ?? throw new BookNotFoundException(id);

        await _books.DeleteAsync(book.Id, ct);
    }

    private static BookDto ToDto(Book b) => new()
    {
        Id = b.Id,
        BookName = b.BookName,
        Authors = b.Authors.Select(a => new AuthorDto { Id = a.Id, FirstName = a.FirstName, LastName = a.LastName }).ToList(),
        Genre = b.Genre,
        AvailableLanguage = b.AvailableLanguage,
        Price = b.Price,
        ImagePath = b.ImagePath,
        SmallDescription = b.SmallDescription,
        DateOfPublishment = b.DateOfPublishment,
        HasPdf = b.PdfBook is not null
    };
}
