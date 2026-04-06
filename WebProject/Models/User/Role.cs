namespace WebProject.Models;

public class Role
{
    public string Name { get; set; }
    public ICollection<int> Permissions { get; set; } = [];
    public IReadOnlyCollection<User> Users { get; set; } = [];
}
