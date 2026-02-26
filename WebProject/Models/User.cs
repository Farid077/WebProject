using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(16, ErrorMessage = "Username must not exceed 16 characters"), MinLength(3, ErrorMessage = "Username must contain minimum 3 characters")]
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
