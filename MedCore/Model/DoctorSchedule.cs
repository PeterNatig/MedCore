using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public class DoctorSchedule : BaseEntity
    {
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsBooked { get; set; }

        public Appointment Appointment { get; set; }
    }
}
