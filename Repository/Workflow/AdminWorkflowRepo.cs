using MedCore.Data;
using MedCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Repository.Workflow;

public class AdminWorkflowRepo : IAdminWorkflowRepo
{
    private readonly MedCoreDbContext _context;

    public AdminWorkflowRepo(MedCoreDbContext context)
    {
        _context = context;
    }

    public Task<List<Specialty>> GetSpecialtiesAsync()
    {
        return _context.Specialties
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Doctor>> GetDoctorsAsync()
    {
        return _context.Doctors
            .OrderBy(d => d.FullName)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Patient>> GetPatientsAsync()
    {
        return _context.Patients
            .OrderBy(p => p.FullName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddSpecialtyAsync(Specialty specialty)
    {
        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteSpecialtyAsync(string specialtyId)
    {
        var specialty = await _context.Specialties.FirstOrDefaultAsync(s => s.Id == specialtyId);
        if (specialty is null)
        {
            return false;
        }

        specialty.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Medication>> GetMedicationsAsync()
    {
        return _context.Medications
            .OrderBy(m => m.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddMedicationAsync(Medication medication)
    {
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteMedicationAsync(string medicationId)
    {
        var medication = await _context.Medications.FirstOrDefaultAsync(m => m.Id == medicationId);
        if (medication is null)
        {
            return false;
        }

        medication.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // Aggregate count queries — SELECT COUNT(*) at DB level
    public Task<int> GetSpecialtiesCountAsync()
    {
        return _context.Specialties.CountAsync();
    }

    public Task<int> GetDoctorsCountAsync()
    {
        return _context.Doctors.CountAsync();
    }

    public Task<int> GetPatientsCountAsync()
    {
        return _context.Patients.CountAsync();
    }
}
