namespace WebProject.ViewModels;

public class UserManagementVM
{
    public string Username { get; set; } = "-";
    public string Role { get; set; } = "-";
    public bool IsActive { get; set; } = false;
    public DateTime CreatedTime { get; set; }
    public IReadOnlyCollection<IssueCardVM> ReportedIssues { get; set; } = [];
    public IReadOnlyCollection<IssueCardVM> AssignedIssues { get; set; } = [];
}
