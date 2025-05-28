using EruMobil.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EruMobil.Persistence.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(d => d.Id); // Eğer Id varsa, anahtar olarak belirt

            builder.HasOne(d => d.User)
                   .WithMany(u => u.Devices)
                   .HasForeignKey(d => d.UserId);

            builder.Property(d => d.DeviceName)
                   .HasMaxLength(100)
                   .IsRequired(); // Örnek alan

            // Diğer property konfigürasyonlarını buraya ekleyebilirsin
        }
    }
}
