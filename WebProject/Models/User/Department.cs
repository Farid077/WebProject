namespace WebProject.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
}
