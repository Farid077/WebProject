namespace WebProject.Models;

public class IssueCategory : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Issue>? Issues { get; set; }
}
