namespace WebProject.ViewModels;

public class RoleManagementVM
{
    public string Name { get; set; }
    public ICollection<Dictionary<string, string>> Permissions { get; set; } = [];
    public ICollection<RoleUsersVM> Users { get; set; } = [];
}
