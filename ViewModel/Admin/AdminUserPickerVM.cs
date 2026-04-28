namespace ViewModel.Admin;

public class AdminUserPickerVM
{
    public IReadOnlyList<AdminUserOptionVM> Doctors { get; set; } = [];
    public IReadOnlyList<AdminUserOptionVM> Patients { get; set; } = [];
}
