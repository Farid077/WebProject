using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels;

public class UserUpdateViewModel
{
    //[Required(ErrorMessage = "Username cannot be empty!"), MaxLength(16, ErrorMessage = "Username must not exceed 16 characters!"), MinLength(3, ErrorMessage = "Username must contain minimum 3 characters!")]
    //public string Username { get; set; }

    [MinLength(8, ErrorMessage = "Password must contain minimum 8 characters")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    public string? ConfirmPassword { get; set; }
}
