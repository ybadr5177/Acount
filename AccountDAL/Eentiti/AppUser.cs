using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class AppUser:IdentityUser
    {
        public string? ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string FristNames { get; set; }
        public string LastName { get; set; }
        public string FullNames { get; set; }
        public int AddressId { get; set; }

        public Address Address { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
    }
}
