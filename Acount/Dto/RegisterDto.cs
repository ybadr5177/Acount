using AccountDAL.Eentiti;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Acount.Dto
{
    public class RegisterDto
    {
        public string? ProfilePicture { get; set; }
        public IFormFile? Imege { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        [Required]

        public string FristNames { get; set; }
        [Required]
        //[RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        //   ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]

        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public string LastName { get; set; }
        public string FullNames { get; set; }

        public string UserName { get; set; }

    }
}
