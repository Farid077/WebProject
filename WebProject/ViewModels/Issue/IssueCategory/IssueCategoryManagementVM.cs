namespace WebProject.ViewModels;

public class IssueCategoryManagementVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<string> SubCategories { get; set; }
}
