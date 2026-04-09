namespace WebProject.ViewModels;

public class IssueCategoryUpdateVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public HashSet<string> SubCategories { get; set; }
}
