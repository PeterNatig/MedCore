namespace MedCore.Model
{
    public class Doctor : User
    {
        public string? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        public string? LicenseNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public decimal? HourRate { get; set; }

        public ICollection<DoctorSchedule>? Schedules { get; set; }

    }
}
