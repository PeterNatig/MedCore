using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class UserConfiguration : BaseConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.ToTable("Users", t => t.HasCheckConstraint("DOB", "[DateOfBirth] < GETDATE()"));
            builder.HasIndex(e => e.NationalId).IsUnique();
            builder.HasDiscriminator<string>("UserType").HasValue<Doctor>("Doctor").HasValue<Patient>("Patient");
            builder.Property(u => u.Gender).IsRequired().HasConversion<string>();

        }
    }
}
