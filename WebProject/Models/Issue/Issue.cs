namespace WebProject.Models;

public class Issue : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public IssueCategory? Category { get; set; }
}
