using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess
{
    public static class RoleSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var passwordHasher = new PasswordHasher<User>();

            var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
            if (adminUser == null)
            {
                User admin = new User
                {
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    PhoneNumber = "0898647382",
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true
                };
                admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123@");
                await userManager.CreateAsync(admin, "Admin123@");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            string userEmail = "aykut@gmail.com";
            string user2Email = "ivan@gmail.com";
            var user2User = await userManager.FindByEmailAsync(user2Email);
            var userUser = await userManager.FindByEmailAsync(userEmail);
            if (userUser == null)
            {
                User user1 = new User
                {
                    UserName = "Aykut",
                    Email = userEmail,
                    FirstName = "Aykut",
                    LastName = "Yamak",
                    EmailConfirmed = true
                };
                user1.PasswordHash = passwordHasher.HashPassword(user1, "Aykut123@");
                await userManager.CreateAsync(user1,"Aykut123@");
                await userManager.AddToRoleAsync(user1, "User");
            }
            if (user2User == null)
            {
                User user2 = new User
                {
                    UserName = "Ivan",
                    Email = user2Email,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    EmailConfirmed = true
                };
                user2.PasswordHash = passwordHasher.HashPassword(user2, "Ivan123@");
                await userManager.CreateAsync(user2, "Ivant123@");
                await userManager.AddToRoleAsync(user2, "User");
            }
        }
    }
}
