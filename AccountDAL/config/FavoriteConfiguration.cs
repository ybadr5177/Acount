using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.config
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorites>
    {
        public void Configure(EntityTypeBuilder<Favorites> builder)
        {
            builder
                   .HasOne(f => f.Product)
                    .WithMany() // أو WithMany(p => p.Favorites) لو عايز تعرف كل المفضلات في الـ Product
                 .HasForeignKey(f => f.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                   builder
                   .HasIndex(f => f.UserEmail); // لتسريع البحث لكل مستخدم
        }
    }
}
