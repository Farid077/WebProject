using Microsoft.Identity.Client;
using WebProject.Models;

namespace WebProject.ViewModels;

public class IssueManagementVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; } = "-";
    public DateOnly CreatedTime { get; set; }
    public string Category { get; set; } = "-";
    public ICollection<User> Users { get; set; } = [new() { Username = "s", PasswordHash = "asda" }];
}
