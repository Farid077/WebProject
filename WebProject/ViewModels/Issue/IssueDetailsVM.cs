namespace WebProject.ViewModels;

public class IssueDetailsVM
{
    public string Category { get; set; } = "-";
    public string SubCategory { get; set; } = "-";
    public string Description { get; set; } = "";
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
