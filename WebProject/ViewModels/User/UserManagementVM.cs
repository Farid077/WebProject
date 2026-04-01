namespace WebProject.ViewModels;

public class UserManagementVM
{
    public string Username { get; set; } = "-";
    public string Role { get; set; } = "-";
    public bool IsActive { get; set; } = false;
    public DateOnly CreatedTime { get; set; }
}
