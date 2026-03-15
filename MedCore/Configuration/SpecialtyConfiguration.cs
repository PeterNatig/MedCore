using MedCore.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class SpecialtyConfiguration:BaseConfiguration<Specialty>
    {
        public override void Configure(EntityTypeBuilder<Specialty> builder)
        {
            base.Configure(builder);
        }
    }
}
