using WebProject.Models;

namespace WebProject.ViewModels;

public class IssueManagementVM
{
    public int Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Status { get; set; }
    public string Urgency {  get; set; }
    public string ReporterName { get; set; }
    public string AssigneeName { get; set; }

}
