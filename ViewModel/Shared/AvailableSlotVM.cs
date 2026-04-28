namespace ViewModel.Shared;

public class AvailableSlotVM
{
    public string ScheduleId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
}
