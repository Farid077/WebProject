using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProject.Models;

namespace WebProject.Configurations;

public class UrgencyConfiguration : IEntityTypeConfiguration<Urgency>
{
    public void Configure(EntityTypeBuilder<Urgency> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.Time)
            .HasDefaultValue(0);
    }
}
