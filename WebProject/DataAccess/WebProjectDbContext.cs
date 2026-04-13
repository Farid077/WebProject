using Microsoft.EntityFrameworkCore;
using WebProject.Models;

namespace WebProject.DataAccess
{
    public class WebProjectDbContext : DbContext
    {
        public WebProjectDbContext(DbContextOptions<WebProjectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueCategory> IssueCategories { get; set; }
        public DbSet<Urgency> Urgencies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(WebProjectDbContext).Assembly);

            base.OnModelCreating(builder);
        }

    }
}
