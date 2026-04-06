namespace WebProject.Models;

public class Issue : BaseEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public IssueStatuses Status { get; set; } = IssueStatuses.Pending;
    public int? CategoryId { get; set; }
    public IssueCategory? Category { get; set; }
    public int? UrgencyId { get; set; }
    public Urgency? Urgency { get; set; }
    public string? ReporterId { get; set; }
    public User? Reporter { get; set; }
    public string? AssigneeId { get; set; }
    public User? Assignee { get; set; }
}
