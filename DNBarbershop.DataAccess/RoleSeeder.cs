using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

            string adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminEmail == null)
            {
                User user = new User
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Admin123@");
                await userManager.AddToRoleAsync(user, "Admin");
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
                }
            }
        }
    }
}
