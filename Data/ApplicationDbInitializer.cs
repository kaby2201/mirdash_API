using System;
using backend.Models;
using System.Linq;
using System.Net.Mime;
using backend.Helpers;
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
            // for (var i = 1; i <= 4; i++)
            // {
                    var robot = new Robot {
                    Hostname = $"MiR_S274-{0 + 0:D2}",
                    BasePath = $"http://127.0.0.1:5003/api/v2.0.0",
                    Token = "YWRtaW46M2I0ZjgzMDBjOGM1ZDkwNjc4YjdkYzNmNGQ1OWY5MGFkZTEwODIzNmFiNDEwNTA1YTlkNTk3OWUxZjk1NGQ1Zg==",
                };
                await context.Robots.AddAsync(robot);
            //}
            
            await context.SaveChangesAsync();
            var mission = new Mission
            {
                RobotId = context.Robots.First().Id,
                Guid = "mirconst-guid-0000-0001-actionlist00",
                Name = "Move",
                Url = "/v2.0.0/missions/mirconst-guid-0000-0001-actionlist00"
            };
            await context.Missions.AddAsync(mission);

            var quadItem = new MissionQueueRequest
            {
                Mission = mission,
                MissionId = mission.Id,
                Robot = context.Robots.First(),
                RobotId = context.Robots.First().Id,
                Name = "Food route",
                Guid = "53E6F670-7B68-495E-AA73-D27327B55006",
                Description = "Mission added by initializer, go get me some food"
            };
            
            await context.MissionQueueRequests.AddAsync(quadItem);
            await context.SaveChangesAsync();
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
