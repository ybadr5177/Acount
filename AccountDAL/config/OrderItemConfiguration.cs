using AccountDAL.Eentiti.Order_Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(orderItem => orderItem.ProductItemOrdered, NP =>
            {
                NP.WithOwner();
            });
            builder.Property(orderItem => orderItem.Price)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
