using ViewModel.Admin;

namespace Service.Workflow;

public interface IAdminWorkflowService
{
    Task<AdminDashboardVM> GetDashboardAsync();
    Task<SpecialtyManagementVM> GetSpecialtiesAsync();
    Task<AdminUserPickerVM> GetUserPickersAsync();
    Task AddSpecialtyAsync(string name, string description);
    Task<bool> DeleteSpecialtyAsync(string specialtyId);
    Task<MedicationManagementVM> GetMedicationsAsync();
    Task AddMedicationAsync(string name, string genericName);
    Task<bool> DeleteMedicationAsync(string medicationId);
}
