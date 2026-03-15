using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.OwnsOne(x => x.Allergies, y =>
            {
                y.Property(e => e.Details).HasColumnName("Allergies");
            }
            );
            builder.OwnsOne(x => x.ChronicConditions, y =>
            {
                y.Property(e => e.Details).HasColumnName("Chronic");
            }
            );
            builder.Property(e => e.BloodType).HasConversion<string>();
        }
    }
}
