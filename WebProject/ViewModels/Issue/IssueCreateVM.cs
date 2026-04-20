namespace WebProject.ViewModels;

public class IssueCreateVM
{
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string? Description { get; set; } = "";
    public string Urgency { get; set; }
    public IReadOnlyCollection<string> Urgencies { get; set; } = [];
    public Dictionary<string, HashSet<string>> Categories { get; set; } = [];
}
