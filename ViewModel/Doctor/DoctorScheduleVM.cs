using System.ComponentModel.DataAnnotations;

namespace ViewModel.Doctor;

public class DoctorScheduleVM
{
    public IReadOnlyList<DoctorScheduleItemVM> Schedules { get; set; } = [];

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}

public class DoctorScheduleItemVM
{
    public string ScheduleId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
    public bool IsPast { get; set; }
}
