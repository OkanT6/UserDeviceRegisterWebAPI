using EruMobil.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Persistence.Configurations
{
    public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.ToTable("ApiKeys");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Key)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(a => a.CreatedAt)
                   .IsRequired();

            builder.Property(a => a.IsRevoked) // ✅ Bu eklenmeli
                   .IsRequired();

            builder.Property(a => a.ExpiresAt) // ✅ Bu da kullanılmalı
                   .IsRequired();

            builder.HasOne(a => a.Device)
                   .WithMany(d => d.ApiKeys)
                   .HasForeignKey(a => a.DeviceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }



    }
}
