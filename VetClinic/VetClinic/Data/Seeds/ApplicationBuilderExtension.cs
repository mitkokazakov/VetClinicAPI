using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Data.Models;

namespace VetClinic.Data.Seeds
{
    public static class ApplicationBuilderExtension
    {

        public static async Task<IApplicationBuilder> SeedDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var services = serviceScope.ServiceProvider;

            await RoleSeeder(services);
            await SeedUsers(services);

            return app;
        }
        private static async Task RoleSeeder(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = {"Admin", "User"};

            IdentityResult roleResult;

            foreach (var role in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);

                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByNameAsync
                           ("mitko@tpp2.com") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "mitko@tpp2.com";
                user.Email = "mitko@tpp2.com";
                user.FirstName = "Dimitar";
                user.LastName = "Kazakov";

                var result = await userManager.CreateAsync
                (user, "88888888");

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                                        "Admin").Wait();
                }
            }
        }
    }
}
