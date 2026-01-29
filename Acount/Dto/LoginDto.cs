using System.ComponentModel.DataAnnotations;

namespace Acount.Dto
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
