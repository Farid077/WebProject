using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class UserLoginViewModel
{
    [Required(ErrorMessage = "Enter your Username"), MaxLength(16, ErrorMessage = "Username must not exceed 16 characters"), MinLength(3, ErrorMessage = "Username must contain minimum 3 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Enter your password")]
    public string Password { get; set; }
}
