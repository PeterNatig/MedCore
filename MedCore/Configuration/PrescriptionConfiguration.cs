using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class PrescriptionConfiguration : BaseConfiguration<Prescription>
    {
        public override void Configure(EntityTypeBuilder<Prescription> builder)
        {
            base.Configure(builder);
            builder.HasIndex(p => new { p.AppointmentId, p.MedicationId }).IsUnique();
            builder.HasOne(p => p.Medication).WithMany(m => m.Prescriptions).HasForeignKey(p => p.MedicationId);
            builder.HasOne(p => p.Appointment).WithMany(m => m.Prescriptions).HasForeignKey(p => p.AppointmentId);

        }
    }
}
