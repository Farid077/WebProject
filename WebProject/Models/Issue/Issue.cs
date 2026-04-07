namespace WebProject.Models;

public class Issue : BaseEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = IssueStatuses.Pending.ToString();
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public int? UrgencyId { get; set; }
    public Urgency? Urgency { get; set; }
    public string? ReporterId { get; set; }
    public User? Reporter { get; set; }
    public string? AssigneeId { get; set; }
    public User? Assignee { get; set; }
}
