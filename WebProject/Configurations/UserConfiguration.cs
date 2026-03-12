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
            .HasMaxLength(16);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId);

        //builder.HasData(
        //    new User
        //    {
        //        Username = "admin",
        //        PasswordHash = "AQAAAAIAAYagAAAAEMzFSuS1OlYOV9tcCWe4vgqVLIjBwbr=="
        //    });
    }
}
