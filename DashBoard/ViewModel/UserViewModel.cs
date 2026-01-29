using AccountDAL.Eentiti;
using System.ComponentModel.DataAnnotations;

namespace DashBoard.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string? ImegeName { get; set; }
        public IFormFile? Imege { get; set; }
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        [Required]

        public string FristNames { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
