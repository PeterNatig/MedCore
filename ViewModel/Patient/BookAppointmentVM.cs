using System.ComponentModel.DataAnnotations;
using ViewModel.Shared;

namespace ViewModel.Patient;

public class BookAppointmentVM
{
    public string DoctorId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public IReadOnlyList<AvailableSlotVM> AvailableSlots { get; set; } = [];

    [Required]
    public string SelectedScheduleId { get; set; } = string.Empty;
}
