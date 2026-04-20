namespace WebProject.ViewModels;

public class HomePageVM
{
    public List<IssueCardVM> Issues { get; set; } = [];
    public IssueCreateVM IssueCreateForm { get; set; }
}
