using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebProject.Models;

namespace WebProject.Configurations
{
    public class IssueConfiguration : IEntityTypeConfiguration<Issue>
    {
        public void Configure(EntityTypeBuilder<Issue> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(16);
            
            builder.Property(x => x.Subtitle)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(x => x.Description)
                .HasMaxLength(64)
                .IsRequired(false);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Issues)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
