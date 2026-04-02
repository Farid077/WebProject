namespace WebProject.ViewModels;

public class IssueDetailsVM
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; } = "";
    public string Category { get; set; } = "-";
    public DateOnly CreatedTime { get; set; }
    public DateOnly? UpdatedTime { get; set; }
}
