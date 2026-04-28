using ViewModel.Doctor;

namespace Service.Workflow;

public interface IDoctorWorkflowService
{
    Task<DoctorDashboardVM?> GetDashboardAsync(string doctorEmail);
    Task<DoctorScheduleVM?> GetSchedulesAsync(string doctorEmail);
    Task<bool> AddScheduleAsync(string doctorEmail, DateTime startTime, DateTime endTime);
    Task<bool> DeleteScheduleAsync(string doctorEmail, string scheduleId);
    Task<DoctorPatientsVM?> GetPatientsAsync(string doctorEmail);
    Task<ConsultationVM?> GetConsultationAsync(string doctorEmail);
    Task<bool> AddPrescriptionAsync(string doctorEmail, ConsultationVM model);
    Task<bool> ConfirmAppointmentAsync(string doctorEmail, string appointmentId);
    Task<bool> RejectAppointmentAsync(string doctorEmail, string appointmentId);
}
