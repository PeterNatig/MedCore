using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public class Enums
    {
        public enum Gender { Unknown = 0, Male = 1, Female = 2, Other = 3 }
        public enum BloodType { Unknown = 0, APositive, ANegative, BPositive, BNegative, OPositive, ONegative, ABPositive, ABNegative }
        public enum AppointmentStatus { Pending = 0, Confirmed = 1, Completed = 2, Cancelled = 3 }

    }
}
