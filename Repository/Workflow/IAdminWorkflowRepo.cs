using MedCore.Model;

namespace Repository.Workflow;

public interface IAdminWorkflowRepo
{
    Task<List<Specialty>> GetSpecialtiesAsync();
    Task<List<Doctor>> GetDoctorsAsync();
    Task<List<Patient>> GetPatientsAsync();
    Task AddSpecialtyAsync(Specialty specialty);
    Task<bool> DeleteSpecialtyAsync(string specialtyId);
    Task<List<Medication>> GetMedicationsAsync();
    Task AddMedicationAsync(Medication medication);
    Task<bool> DeleteMedicationAsync(string medicationId);

    // Dashboard aggregate queries
    Task<int> GetSpecialtiesCountAsync();
    Task<int> GetDoctorsCountAsync();
    Task<int> GetPatientsCountAsync();
}
