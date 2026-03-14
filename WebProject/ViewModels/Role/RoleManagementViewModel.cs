namespace WebProject.ViewModels;

public class RoleManagementViewModel
{
    public string Name { get; set; }
    public ICollection<Dictionary<string, string>> Permissions { get; set; } = [];
    public ICollection<RoleUsersViewModel> Users { get; set; } = [];
}
