using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo
{
    public class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider services)
        {
            var roleManager = services
            .GetRequiredService<RoleManager<IdentityRole>>();

            await EnsureRolesAsync(roleManager);

            var userManager = services
            .GetRequiredService<UserManager<ApplicationUser>>(
            );

            await EnsureTestAdminAsync(userManager);
        }

        /// <summary>EnsureRolesAsync is a method in SeedData class.
        /// <para>receive a role and check if exists. if it doesn't creates the role. <see cref="AspNetCoreTodo.SeedData"/> for information about output statements.</para>
        /// <seealso cref="SeedData"/>
        /// <returns>boolean value</returns>
        /// </summary>
        private static async Task EnsureRolesAsync(
                RoleManager<IdentityRole> roleManager)
        {
            var alreadyExists = await roleManager
            .RoleExistsAsync(Constants.AdministratorRole);
            if (alreadyExists) return;
            await roleManager.CreateAsync(
            new IdentityRole(Constants.AdministratorRole));
        }

        /// <summary>EnsureTestAdminAsync class in SeedData class.
        /// <para>receive a user manager check if user exists if doesn't create it. <see cref="AspNetCoreTodo.SeedData"/> for information about output statements.</para>
        /// <seealso cref="SeedData"/>
        /// </summary>
        private static async Task EnsureTestAdminAsync(
            UserManager<ApplicationUser> userManager)
        {
            var testAdmin = await userManager.Users
            .Where(x => x.UserName == "victor@admin.local")
            .SingleOrDefaultAsync();
            if (testAdmin != null) return;
            testAdmin = new ApplicationUser
            {
                UserName = "victor@admin.local",
                Email = "victor@admin.local"
            };
            await userManager.CreateAsync(
            testAdmin, "NotSecure123!!");
            await userManager.AddToRoleAsync(
            testAdmin, Constants.AdministratorRole);
        }
    }
}
