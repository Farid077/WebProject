namespace WebProject.ViewModels;

public class IssueCardVM
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Urgency { get; set; }
    public string ReporterName { get; set; }
    public string AssigneeName { get; set; }
    public DateTime ReportedTime { get; set; }
}
