using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.Property(d => d.LicenseNumber).IsRequired();

            builder.Property(d => d.HourRate).IsRequired().HasColumnType("decimal(18,2)");
            builder.HasOne(e=>e.Specialty).WithMany(e=>e.Doctors).HasForeignKey(e=>e.SpecialtyId);

        }
    }
}
