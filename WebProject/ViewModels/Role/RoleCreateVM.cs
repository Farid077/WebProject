using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class RoleCreateVM
{
    [Required(ErrorMessage = "Enter Role Name"), MaxLength(16, ErrorMessage = "Role name must not exceed 16 characters"), MinLength(2, ErrorMessage = "Role name must contain minimum 2 characters")]
    public string RoleName {  get; set; }
    public List<Pair> Permissions { get; set; } = [];
    public IEnumerable<string> PageOptions { get; set; } = [];
    public IEnumerable<string> AccessOptions { get; set; } = [];
}

public class Pair
{
    [Required(ErrorMessage = "Select Page")]
    public string Page {  get; set; }

    [Required(ErrorMessage = "Select Accessibility")]
    public string Access { get; set; }
}
