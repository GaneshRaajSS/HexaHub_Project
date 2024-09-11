using System.ComponentModel.DataAnnotations;

namespace Hexa_Hub.Models
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string UserMail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
