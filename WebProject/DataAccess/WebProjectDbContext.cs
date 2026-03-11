using Microsoft.EntityFrameworkCore;
using WebProject.Models;

namespace WebProject.DataAccess
{
    public class WebProjectDbContext : DbContext
    {
        public WebProjectDbContext(DbContextOptions<WebProjectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(WebProjectDbContext).Assembly);

            base.OnModelCreating(builder);
        }

    }
}
