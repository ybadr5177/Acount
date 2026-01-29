using AccountDAL.Eentiti;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.config
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options) 
        {

        }
        public DbSet<Address> Address { get; set; }
        public DbSet<ShippingAddress> ShippingAddress { get; set; }

    }
}
