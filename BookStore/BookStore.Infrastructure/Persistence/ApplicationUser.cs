using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Infrastructure.Persistence;

public class ApplicationUser : IdentityUser
{
    public ICollection<PurchasedBook> PurchasedBooks { get; set; } = new List<PurchasedBook>();
}
