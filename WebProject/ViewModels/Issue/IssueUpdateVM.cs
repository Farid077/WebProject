using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class IssueUpdateVM
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }

    // change it to 128 after adding new migration
    [MaxLength(64, ErrorMessage = "Description cannot be include more than 64 characters.")]
    public string? Description { get; set; } = "";
    public string Status { get; set; }
    public string Urgency { get; set; }
    public string? AssigneeName { get; set; }
    public IReadOnlyCollection<string> Statuses { get; set; } = [];
    public IReadOnlyCollection<string> Urgencies { get; set; } = [];
    public IReadOnlyCollection<string> Users { get; set; } = [];
    public Dictionary<string, HashSet<string>> Categories { get; set; } = [];
}
