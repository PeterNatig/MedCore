using static MedCore.Model.Enums;

namespace MedCore.Model
{
    public class Patient : User
    {
        public BloodType? BloodType { get; set; }

        public Allergies? Allergies { get; set; } = new Allergies();
        public ChronicConditions? ChronicConditions { get; set; } = new ChronicConditions();

        public ICollection<Appointment>? Appointments { get; set; }
    }

}
