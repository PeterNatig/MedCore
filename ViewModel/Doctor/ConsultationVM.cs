using System.ComponentModel.DataAnnotations;

namespace ViewModel.Doctor;

public class ConsultationVM
{
    [Required]
    public string AppointmentId { get; set; } = string.Empty;

    [Required]
    public string MedicationId { get; set; } = string.Empty;

    [Required]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    public string Frequency { get; set; } = string.Empty;

    public IReadOnlyList<MedicationOptionVM> Medications { get; set; } = [];
    public IReadOnlyList<DoctorPatientItemVM> OpenAppointments { get; set; } = [];
}

public class MedicationOptionVM
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
