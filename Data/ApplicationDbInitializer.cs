using System;
using backend.Models;
using System.Linq;
using System.Net.Mime;
using backend.Helpers;
using backend.Models.Robots;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class ApplicationDbInitializer
    {
        private static void CreateUsersAndRoles(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create administrator roles
            var adminRole = new IdentityRole(Role.Admin);
            roleManager.CreateAsync(adminRole).Wait();

            // Create a user role
            var userRole = new IdentityRole(Role.User);
            roleManager.CreateAsync(userRole).Wait();

            // Create administrator users, username must be lowercase
            var admin = new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "Strator",
                UserName = "admin",
                Email = "admin@mail.com",
                Role = Role.Admin
            };
            userManager.CreateAsync(admin, "Password1.").Wait();
            userManager.AddToRoleAsync(admin, Role.Admin).Wait();

            // Create user, username must be lowercase
            var user = new ApplicationUser
            {
                FirstName = "User",
                LastName = "Nami",
                UserName = "user",
                Email = "user@mail.com",
                Role = Role.User
            };
            userManager.CreateAsync(user, "Password1.").Wait();
            userManager.AddToRoleAsync(user, Role.User).Wait();
        }

        public static async void CreateRobotAsync(ApplicationDbContext context)
        {
            var robot = new Robot
            {
                Hostname = "MiR_S274-X1",
                BasePath = "http://127.0.0.1:5003/api/v2.0.0",
                SerialNumber = "180200011105003",
                Position = new Position(),
                Velocity = new Velocity(),
                Token =
                    "YWRtaW46M2I0ZjgzMDBjOGM1ZDkwNjc4YjdkYzNmNGQ1OWY5MGFkZTEwODIzNmFiNDEwNTA1YTlkNTk3OWUxZjk1NGQ1Zg==",
            };
            await context.Robots.AddAsync(robot);
            await context.SaveChangesAsync();
            
            // var mission = new Mission
            // {
            //     Robot = context.Robots.First(),
            //     Guid = "mirconst-guid-0000-0001-actionlist00",
            //     Name = "Move",
            //     Url = "/missions/mirconst-guid-0000-0001-actionlist00"
            // };
            // await context.Missions.AddAsync(mission);
            
            //await context.SaveChangesAsync();
        }

        public static void Init(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, bool development)
        {
            // Run migrations and add users if we're not in development mode
            if (!development)
            {
                context.Database.Migrate();

                // Only create users if no users exist
                if (!context.ApplicationUsers.Any())
                    CreateUsersAndRoles(userManager, roleManager);

                return;
            }

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            CreateRobotAsync(context);
            CreateUsersAndRoles(userManager, roleManager);
            context.SaveChangesAsync();
        }
    }
}