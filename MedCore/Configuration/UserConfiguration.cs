using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", t => t.HasCheckConstraint("DOB", "[DateOfBirth] < GETUTCDATE()"));
            builder.HasIndex(e => e.NationalId).IsUnique();
            builder.HasDiscriminator<string>("UserType").HasValue<Doctor>("Doctor").HasValue<Patient>("Patient");
            builder.Property(u => u.Gender).IsRequired().HasConversion<string>();

            builder.Property(e => e.Version).IsRowVersion();
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
            builder.HasQueryFilter(e => !e.IsDeleted);


        }
    }
}
