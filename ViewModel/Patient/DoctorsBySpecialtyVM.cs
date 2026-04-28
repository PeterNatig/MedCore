using ViewModel.Shared;

namespace ViewModel.Patient;

public class DoctorsBySpecialtyVM
{
    public string SpecialtyId { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public IReadOnlyList<DoctorCardVM> Doctors { get; set; } = [];
}
