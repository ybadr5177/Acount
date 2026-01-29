using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Eentiti.Order_Aggregate;
using AccountDAL.Eentiti.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.config
{
    public class CozaStoreContext : DbContext
    {
        public CozaStoreContext(DbContextOptions<CozaStoreContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());





        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SliderItem> SliderItems { get; set; }
        public DbSet<opponent> opponents { get; set; }
        public DbSet<DifferentSize> DifferentSizes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<SizeName> SizeNames { get; set; }


        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TermsAndConditions> TermsAndConditionses { get; set; }
        public DbSet<ContactUS> contactus { get; set; }
        public DbSet<DeliveryCountry> DeliveryCountrys { get; set; }

        public DbSet<DeliveryCost> DeliveryCosts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Message> Messages { get; set; }






    }
}
