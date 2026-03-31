namespace WebProject.Models;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string? RoleId { get; set; }
    public Role? Role { get; set; }
}
