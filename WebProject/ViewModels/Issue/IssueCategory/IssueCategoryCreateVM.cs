namespace WebProject.ViewModels;

public class IssueCategoryCreateVM
{
    public string Name { get; set; }
    public HashSet<string> SubCategories { get; set; }
}
