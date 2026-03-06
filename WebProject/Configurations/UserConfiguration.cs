using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProject.Models;

namespace WebProject.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Username);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasData(
            new User
            {
                Username = "admin",
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "admin")
            });
    }
}
