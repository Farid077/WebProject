namespace WebProject.ViewModels;

public class IssueManagementVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public DateOnly CreatedTime { get; set; }
    public string Category { get; set; } = "-";
}
