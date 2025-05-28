using EruMobil.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.BusinessIdentifier)
            .HasMaxLength(10)
            .IsFixedLength()
            .IsRequired(true)
            .HasColumnType("character(10)");

            // Ek güvenlik için (SQL tarafında zaten sınırlanıyor ama uygulama içinde de kural koymak istersen):
            //builder.HasCheckConstraint("CK_User_StudentNumber_Length", "LEN(StudentNumber) = 10");
        }
    }
}
