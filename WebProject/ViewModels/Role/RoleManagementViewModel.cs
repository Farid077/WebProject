namespace WebProject.ViewModels;

public class RoleManagementViewModel
{
    public string Name { get; set; }
    public ICollection<int> Permissions { get; set; } = [];
    public ICollection<UserManagementViewModel> Users { get; set; } = [];
}
