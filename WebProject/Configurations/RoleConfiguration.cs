using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProject.Models;

namespace WebProject.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Name);

        builder.Property(x => x.Name)
            .HasMaxLength(16);

        builder.Property(x => x.Permissions)
            .IsRequired();

        builder.HasData(
            new Role
            {
                Name = "SuperAdmin",
                Permissions = (ICollection<int>)Enum.GetValues<Pages>().Select(p => (int)p | (int)PageAccess.Read_Write)
            },
            new Role
            {
                Name = "User"
            });
    }
}
