namespace WebProject.ViewModels;

public class DepartmentManagementVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
