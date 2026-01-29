using AccountDAL.Eentiti;

namespace Acount.Dto
{
    public class UserDto
    {
        public string FristName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string LastName { get; set; }
        public string FullNames { get; set; }
        public Address Address { get; set; }
    }
}
