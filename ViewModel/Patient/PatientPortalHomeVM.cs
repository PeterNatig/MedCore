using ViewModel.Shared;

namespace ViewModel.Patient;

public class PatientPortalHomeVM
{
    public IReadOnlyList<SpecialtyCardVM> Specialties { get; set; } = [];
}
