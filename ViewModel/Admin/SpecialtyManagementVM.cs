using System.ComponentModel.DataAnnotations;

namespace ViewModel.Admin;

public class SpecialtyManagementVM
{
    public IReadOnlyList<SpecialtyItemVM> Specialties { get; set; } = [];
    public IReadOnlyList<AdminUserOptionVM> Doctors { get; set; } = [];
    public IReadOnlyList<AdminUserOptionVM> Patients { get; set; } = [];

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}

public class AdminUserOptionVM
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class SpecialtyItemVM
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
