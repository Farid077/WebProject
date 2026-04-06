namespace WebProject.Models;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string? RoleId { get; set; }
    public Role? Role { get; set; }
    public IReadOnlyCollection<Issue> ReportedIssues { get; set; } = [];
    public IReadOnlyCollection<Issue> AssignedIssues { get; set; } = [];
}
