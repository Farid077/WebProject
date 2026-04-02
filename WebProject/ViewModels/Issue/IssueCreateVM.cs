namespace WebProject.ViewModels;

public class IssueCreateVM
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public ICollection<IssueCategoryManagementVM> Categories { get; set; }
}
