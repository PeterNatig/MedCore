using System.ComponentModel.DataAnnotations;

namespace ViewModel.Admin;

public class MedicationManagementVM
{
    public IReadOnlyList<MedicationItemVM> Medications { get; set; } = [];

    [Required]
    [Display(Name = "Brand Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Generic Name")]
    public string GenericName { get; set; } = string.Empty;
}

public class MedicationItemVM
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
}
