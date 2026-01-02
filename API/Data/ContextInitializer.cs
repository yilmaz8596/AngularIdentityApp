using System;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class ContextInitializer
{
    public static async Task InitializeAsync(AppDbContext context, UserManager<AppUser> userManager)
    {
        if (context.Database.GetPendingMigrations().Count() > 0)
        {
            await context.Database.MigrateAsync();
        }

        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser
            {
                UserName = "john",
                Email = "john@example.com",
                Name = "JOHN",
                EmailConfirmed = true,
                LockoutEnabled = true,
            },
            new AppUser
            {
                UserName = "jane",
                Email = "jane@example.com",
                Name = "JANE",
                EmailConfirmed = true,
                LockoutEnabled = true,
            }
            };
            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Password1!");
            }
        }

        await context.SaveChangesAsync();
    }
}
