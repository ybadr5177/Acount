using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Eentiti.Order_Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.config
{
    public class DifferentSizeConfiguration : IEntityTypeConfiguration<DifferentSize>
    {
        public void Configure(EntityTypeBuilder<DifferentSize> builder)
        { 
            // علاقة DifferentSize → DimensionSize (1:1)
            builder.HasOne(s => s.Dimensions)
                     .WithOne(d => d.DifferentSize)
                     .HasForeignKey<DimensionSize>(d => d.DifferentSizeId);
           
        }
    }
}
