using MedCore.Model;

namespace Repository.Workflow;

public interface IPatientWorkflowRepo
{
    Task<List<Specialty>> GetSpecialtiesAsync();
    Task<List<Doctor>> GetDoctorsBySpecialtyAsync(string specialtyId);
    Task<Doctor?> GetDoctorWithAvailableSlotsAsync(string doctorId, string patientId);
    Task<Patient?> GetPatientByEmailAsync(string email);
    Task<(bool Success, string Message)> BookAppointmentAsync(string patientId, string scheduleId);
    Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId);
    Task<bool> CancelAppointmentAsync(string patientId, string appointmentId);
}
