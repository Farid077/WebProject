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

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(x => x.Description)
                .HasMaxLength(128)
                .HasDefaultValue("");

            builder.Property(x => x.Status)
                .HasDefaultValue(IssueStatuses.Pending.ToString());

            builder.HasOne(x => x.Reporter)
                .WithMany(x => x.ReportedIssues)
                .HasForeignKey(x => x.ReporterId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Assignee)
                .WithMany(x => x.AssignedIssues)
                .HasForeignKey(x => x.AssigneeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Urgency)
                .WithMany(x => x.Issues)
                .HasForeignKey(x => x.UrgencyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
