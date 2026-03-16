using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class RoleCreateViewModel
{
    [Required(ErrorMessage = "Enter Role Name"), MaxLength(16, ErrorMessage = "Role name must not exceed 16 characters"), MinLength(2, ErrorMessage = "Role name must contain minimum 2 characters")]
    public string RoleName {  get; set; }

    [Required]
    public ICollection<Pare> Permissions { get; set; } = [];
    public IEnumerable<string> PageOptions { get; set; } = [];
    public IEnumerable<string> PermissionOptions { get; set; } = [];
}

public class Pare
{
    public string Page {  get; set; }
    public string Permission { get; set; }
}
