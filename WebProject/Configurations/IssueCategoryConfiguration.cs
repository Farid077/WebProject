using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProject.Models;

namespace WebProject.Configurations;

public class IssueCategoryConfiguration : IEntityTypeConfiguration<IssueCategory>
{
    public void Configure(EntityTypeBuilder<IssueCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(16);
    }
}
