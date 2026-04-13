namespace WebProject.ViewModels;

public class UrgencyUpdateVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? Days { get; set; } = 0;
    public int? Hours { get; set; } = 0;
    public int? Minutes { get; set; } = 0;
}
