using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
