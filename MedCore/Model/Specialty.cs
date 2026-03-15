using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public class Specialty:BaseEntity
    {
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}
