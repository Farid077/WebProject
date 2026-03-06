using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Enter Username"), MaxLength(16, ErrorMessage = "Username must not exceed 16 characters"), MinLength(3, ErrorMessage = "Username must contain minimum 3 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Enter password"), MinLength(8, ErrorMessage = "Password must contain minimum 8 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    public string ConfirmPassword { get; set; }
}
