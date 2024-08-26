using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RMS.Data;
using System.Drawing;

namespace RMS.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new GroceryStoreContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<GroceryStoreContext>>()))
        {
            // Look for any users.
            if (context.User.Any())
            {
                return;
            }

            var adminUser = new User
            {
                FName = "Admin",
                LName = "1",
                Email = "admin@gs.com",
                Password = "pass999",
                UserType = "Admin",
            };

            context.Add(adminUser);
            context.SaveChanges();

            // Look for any services.
            if (context.Services.Any())
            {
                return;
            }

            context.Services.AddRange(
                
                new Service
                {
                    Name = "Snacks",
                },
                new Service
                {
                    Name = "Packaged Food",
                },
                new Service
                {
                    Name = "Fruits",
                },
                new Service
                {
                    Name = "Tea and Coffee",
                },
                new Service
                {
                    Name = "Canned Food",
                },
                new Service
                {
                    Name = "Home Baking",
                },
                new Service
                {
                    Name = "Desserts",
                }
            );

            context.SaveChanges();
        }
    }
}