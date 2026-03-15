using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public class Medication : BaseEntity
    {
        public string Name { get; set; }
        public string GenericName { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }

    }
}
