using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class UrgencyCreateVM
{
    public string Name { get; set; }
    public int? Days { get; set; } = 0;
    public int? Hours { get; set; } = 0;
    public int? Minutes { get; set; } = 0;
}
