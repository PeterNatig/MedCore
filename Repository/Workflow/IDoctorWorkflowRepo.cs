using MedCore.Model;

namespace Repository.Workflow;

public interface IDoctorWorkflowRepo
{
    Task<Doctor?> GetDoctorByEmailAsync(string email);
    Task<List<DoctorSchedule>> GetSchedulesAsync(string doctorId);
    Task AddScheduleAsync(DoctorSchedule schedule);
    Task<bool> DeleteScheduleAsync(string doctorId, string scheduleId);
    Task<List<Appointment>> GetDoctorAppointmentsAsync(string doctorId);
    Task<List<Appointment>> GetConfirmedAppointmentsAsync(string doctorId);
    Task<List<Medication>> GetMedicationsAsync();
    Task<bool> AddPrescriptionAsync(Prescription prescription, string doctorId, string appointmentId);
    Task<bool> HasOverlappingScheduleAsync(string doctorId, DateTime startTime, DateTime endTime);
    Task<bool> ConfirmAppointmentAsync(string doctorId, string appointmentId);
    Task<bool> RejectAppointmentAsync(string doctorId, string appointmentId);

    // Dashboard aggregate queries — avoid loading full entities for counting
    Task<int> GetAppointmentsCountAsync(string doctorId);
    Task<int> GetDistinctPatientsCountAsync(string doctorId);
    Task<int> GetPendingAppointmentsCountAsync(string doctorId);
}
