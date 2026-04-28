namespace ViewModel.Doctor;

public class DoctorDashboardVM
{
    public string DoctorName { get; set; } = string.Empty;
    public string? SpecialtyName { get; set; }
    public int SlotsCount { get; set; }
    public int PatientsCount { get; set; }
    public int AppointmentsCount { get; set; }
    public int PendingCount { get; set; }
}
