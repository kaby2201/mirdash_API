using backend.Models;
using backend.Models.Robots;
using backend.Models.Statistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Robot> Robots { get; set; }
        
        public DbSet<Mission> Missions { get; set; }
        
        public DbSet<MissionQueue> MissionQueues { get; set; }
        
        public DbSet<Distance> Distances { get; set; }
    }
}