using BookStore.Domain.Constants;
using BookStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure.Identity;

public static class IdentityInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { BookStoreRoles.Admin, BookStoreRoles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminUser = await userManager.FindByEmailAsync(BookStoreConstants.DefaultAdminEmail);
        if (adminUser is null)
        {
            var user = new ApplicationUser
            {
                UserName = BookStoreConstants.DefaultAdminEmail,
                Email = BookStoreConstants.DefaultAdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, BookStoreConstants.DefaultAdminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, BookStoreRoles.Admin);
        }
    }
}
