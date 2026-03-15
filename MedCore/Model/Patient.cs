using System;
using System.Collections.Generic;
using System.Text;
using static MedCore.Model.Enums;

namespace MedCore.Model
{
    public class Patient : User
    {
        public BloodType BloodType { get; set; }

        public Allergies Allergies { get; set; }
        public ChronicConditions ChronicConditions { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }

}
