namespace ViewModel.Shared;

public class DoctorCardVM
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal HourRate { get; set; }
    public string? Bio { get; set; }
}
