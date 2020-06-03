using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Get our database context from the service provider
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Get the environment so we can check if this is running in development or otherwise
                var environment = services.GetService<IWebHostEnvironment>();

                // Get user and role services so we can initialize those as well
                var um = services.GetRequiredService<UserManager<ApplicationUser>>();
                var rm = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Initialise the database using the initializer from Data/ExampleDbInitializer.cs
                ApplicationDbInitializer.Init(context, um, rm, environment.IsDevelopment());
            }
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}