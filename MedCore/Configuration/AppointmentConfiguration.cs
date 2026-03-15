using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class AppointmentConfiguration : BaseConfiguration<Appointment>
    {
        public override void Configure(EntityTypeBuilder<Appointment> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Status).HasConversion<string>();
            builder.HasOne(e => e.DoctorSchedule).WithOne(e => e.Appointment).HasForeignKey<Appointment>(e => e.ScheduleId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Patient).WithMany(e => e.Appointments).HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
