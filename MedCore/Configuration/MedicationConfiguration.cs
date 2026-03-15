using MedCore.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Configuration
{
    public class MedicationConfiguration:BaseConfiguration<Medication>
    {
        public override void Configure(EntityTypeBuilder<Medication> builder)
        {
            base.Configure(builder);
        }
    }
}
