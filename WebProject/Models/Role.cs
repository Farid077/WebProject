namespace WebProject.Models;

public class Role
{
    public string Name { get; set; }
    public ICollection<int> Permissions { get; set; } = [];
    public IEnumerable<User>? Users { get; set; }
}
