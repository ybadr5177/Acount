using AccountDAL.Eentiti;

namespace Acount.Dto
{
    public class UpdateProfileDto
    {
        public string FristNames { get; set; }
        public string PhoneNumber { get; set; }

        public string UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public IFormFile ProfileFilePicture { get; set; }
        public string LastName { get; set; }
        public string FullNames { get; set; }
    }
}
