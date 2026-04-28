using ViewModel.Patient;

namespace Service.Workflow;

public interface IPatientWorkflowService
{
    Task<PatientPortalHomeVM> GetSpecialtiesAsync();
    Task<DoctorsBySpecialtyVM> GetDoctorsBySpecialtyAsync(string specialtyId);
    Task<BookAppointmentVM?> GetBookingModelAsync(string doctorId, string patientEmail);
    Task<(bool Success, string Message)> BookAppointmentAsync(string patientEmail, string scheduleId);
    Task<MyRecordsVM> GetRecordsAsync(string patientEmail);
    Task<PatientPrescriptionsVM> GetPrescriptionsAsync(string patientEmail);
    Task<bool> CancelAppointmentAsync(string patientEmail, string appointmentId);
}
