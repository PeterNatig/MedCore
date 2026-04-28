using System;
using System.Collections.Generic;
using System.Text;
using static MedCore.Model.Enums;

namespace MedCore.Model
{
    public class Appointment : BaseEntity
    {
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public string ScheduleId { get; set; }
        public DoctorSchedule DoctorSchedule { get; set; }

        public AppointmentStatus Status { get; set; }

        public string CancellationReason { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
