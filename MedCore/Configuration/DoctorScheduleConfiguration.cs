using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class DoctorScheduleConfiguration:BaseConfiguration<DoctorSchedule>
    {
        public override void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            base.Configure(builder);
            builder.ToTable("DoctorSchedule", t => t.HasCheckConstraint("CheckTime", "EndTime>StartTime"));
            builder.HasOne(e => e.Doctor).WithMany(e => e.Schedules).HasForeignKey(e=>e.DoctorId);
        }
    }
}
