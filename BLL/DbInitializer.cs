using BLL;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DbInitializer
    {
        public static async Task Initialize(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var userRole = new IdentityRole { Name = "User" };
            var adminRole = new IdentityRole { Name = "Admin" };


            // Creating Roles
            await roleManager.CreateAsync(userRole);
            await roleManager.CreateAsync(adminRole);


            // Creating users
            var admin = new AppUser
            {
                Email = "admin@gmail.com",
                UserName = "AdamAdmin"//,
                //FirstName = "Adam",
                //LastName = "Adamson"
            };
            string adminPassword = "P@ssw0rd";
            var adminResult = await userManager.CreateAsync(admin, adminPassword);
            var user = new AppUser
            {
                Email = "student@gmail.com",
                UserName = "student@gmail.com"//,
                //FirstName = "Sam",
                //LastName = "Simpson"
            };
            string userPassword = "P@ssw0rd";
            var userResult = await userManager.CreateAsync(user, userPassword);

            if (adminResult.Succeeded && userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRole.Name);
                await userManager.AddToRoleAsync(user, userRole.Name);
                //await userManager.AddToRoleAsync(admin, userRole.Name);
            }
        }

    }
}
