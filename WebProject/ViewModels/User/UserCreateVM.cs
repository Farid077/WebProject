using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class UserCreateVM
{
    [Required(ErrorMessage = "Enter the username"), MaxLength(16, ErrorMessage = "Username must not exceed 16 characters"), MinLength(3, ErrorMessage = "Username must contain minimum 3 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Select the role")]
    public string Role { get; set; }
    public ICollection<string> RoleOptions { get; set; } = [];

    [Required(ErrorMessage = "Enter the password"), MinLength(8, ErrorMessage = "Password must contain minimum 8 characters"), MaxLength(32, ErrorMessage = "Password must not exceed 32 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm the password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    public string ConfirmPassword { get; set; }
}
