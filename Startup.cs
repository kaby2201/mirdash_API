using System;
using AutoMapper;
using System.Text;
using backend.Data;
using backend.Models;
using backend.Helpers;
using backend.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // User password requirements
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Change
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddSpaStaticFiles(options => { options.RootPath = "wwwroot"; });


            // Register the database context as a service. Use SQLite for this
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_configuration.GetConnectionString("ApiDatabase")));

            /*
            else // Database used in all other environments (production etc)
            {
                // Register the database context as a service. Use PostgreSQL server for this
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(_configuration.GetConnectionString("ApiDatabase")));
            }
            */

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();

            services.AddHttpClient();
            services.AddHostedService<RobotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseSpaStaticFiles();
            app.UseSpa(builder =>
             {
                 if (env.IsDevelopment())
                 {
                     builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                 }
             });
        }
    }
}