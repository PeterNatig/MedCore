namespace ViewModel.Doctor;

public class DoctorPatientsVM
{
    public IReadOnlyList<DoctorPatientItemVM> Patients { get; set; } = [];
}

public class DoctorPatientItemVM
{
    public string AppointmentId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsPast { get; set; }
}
