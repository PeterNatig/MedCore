using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Version).IsRowVersion();
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETDATE()");
            builder.HasQueryFilter(e => !e.IsDeleted);

        }
    }
}
