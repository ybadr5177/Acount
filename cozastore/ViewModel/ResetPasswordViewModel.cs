using System.ComponentModel.DataAnnotations;

namespace cozastore.ViewModel
{
    public class ResetPasswordViewModel
    {
        //[Required]
        //[EmailAddress]
        public string? Email { get; set; }

        //[Required]
        public string? Token { get; set; }

        //[Required]
        //[RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        //   ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
