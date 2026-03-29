using BookStore.Application.Interfaces;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("pdf")]
public class PdfController : Controller
{
    private readonly IPdfBookRepository _pdfBooks;
    private readonly IPurchasedBookRepository _purchases;
    private readonly IStorageService _storage;
    private readonly UserManager<ApplicationUser> _userManager;

    public PdfController(
        IPdfBookRepository pdfBooks,
        IPurchasedBookRepository purchases,
        IStorageService storage,
        UserManager<ApplicationUser> userManager)
    {
        _pdfBooks = pdfBooks;
        _purchases = purchases;
        _storage = storage;
        _userManager = userManager;
    }

    [HttpGet("{bookId:int}")]
    public async Task<IActionResult> GetPdf(int bookId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!await _purchases.HasUserPurchasedAsync(user.Id, bookId))
            return Forbid();

        var pdf = await _pdfBooks.GetByBookIdAsync(bookId);
        if (pdf == null) return NotFound();

        var url = await _storage.GetPresignedUrlAsync(pdf.FilePath, TimeSpan.FromSeconds(20));
        return Redirect(url);
    }
}
