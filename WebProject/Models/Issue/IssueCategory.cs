namespace WebProject.Models;

public class IssueCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Issue>? Issues { get; set; }
}
