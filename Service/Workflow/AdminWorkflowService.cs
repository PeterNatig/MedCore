using MedCore.Model;
using Repository.Workflow;
using ViewModel.Admin;

namespace Service.Workflow;

public class AdminWorkflowService : IAdminWorkflowService
{
    private readonly IAdminWorkflowRepo _repo;

    public AdminWorkflowService(IAdminWorkflowRepo repo)
    {
        _repo = repo;
    }

    public async Task<AdminDashboardVM> GetDashboardAsync()
    {
        return new AdminDashboardVM
        {
            SpecialtiesCount = await _repo.GetSpecialtiesCountAsync(),
            DoctorsCount = await _repo.GetDoctorsCountAsync(),
            PatientsCount = await _repo.GetPatientsCountAsync()
        };
    }

    public async Task<SpecialtyManagementVM> GetSpecialtiesAsync()
    {
        var specialties = await _repo.GetSpecialtiesAsync();
        return new SpecialtyManagementVM
        {
            Specialties = specialties.Select(s => new SpecialtyItemVM
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            }).ToList()
        };
    }

    public async Task<AdminUserPickerVM> GetUserPickersAsync()
    {
        var doctors = await _repo.GetDoctorsAsync();
        var patients = await _repo.GetPatientsAsync();
        return new AdminUserPickerVM
        {
            Doctors = doctors.Select(d => new AdminUserOptionVM
            {
                Email = d.Email ?? string.Empty,
                FullName = d.FullName
            }).Where(d => !string.IsNullOrWhiteSpace(d.Email)).ToList(),
            Patients = patients.Select(p => new AdminUserOptionVM
            {
                Email = p.Email ?? string.Empty,
                FullName = p.FullName
            }).Where(p => !string.IsNullOrWhiteSpace(p.Email)).ToList()
        };
    }

    public async Task AddSpecialtyAsync(string name, string description)
    {
        await _repo.AddSpecialtyAsync(new Specialty
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Image = Array.Empty<byte>()
        });
    }

    public Task<bool> DeleteSpecialtyAsync(string specialtyId)
    {
        return _repo.DeleteSpecialtyAsync(specialtyId);
    }

    public async Task<MedicationManagementVM> GetMedicationsAsync()
    {
        var medications = await _repo.GetMedicationsAsync();
        return new MedicationManagementVM
        {
            Medications = medications.Select(m => new MedicationItemVM
            {
                Id = m.Id,
                Name = m.Name,
                GenericName = m.GenericName
            }).ToList()
        };
    }

    public async Task AddMedicationAsync(string name, string genericName)
    {
        await _repo.AddMedicationAsync(new Medication
        {
            Name = name.Trim(),
            GenericName = genericName.Trim()
        });
    }

    public Task<bool> DeleteMedicationAsync(string medicationId)
    {
        return _repo.DeleteMedicationAsync(medicationId);
    }
}
