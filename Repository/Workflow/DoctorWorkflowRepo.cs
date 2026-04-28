using MedCore.Data;
using MedCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Repository.Workflow;

public class DoctorWorkflowRepo : IDoctorWorkflowRepo
{
    private readonly MedCoreDbContext _context;

    public DoctorWorkflowRepo(MedCoreDbContext context)
    {
        _context = context;
    }

    public Task<Doctor?> GetDoctorByEmailAsync(string email)
    {
        return _context.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Email == email);
    }

    public Task<List<DoctorSchedule>> GetSchedulesAsync(string doctorId)
    {
        return _context.DoctorSchedules
            .Where(s => s.DoctorId == doctorId)
            .OrderBy(s => s.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddScheduleAsync(DoctorSchedule schedule)
    {
        _context.DoctorSchedules.Add(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteScheduleAsync(string doctorId, string scheduleId)
    {
        var schedule = await _context.DoctorSchedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.DoctorId == doctorId && !s.IsBooked);
        if (schedule is null)
        {
            return false;
        }

        schedule.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Appointment>> GetDoctorAppointmentsAsync(string doctorId)
    {
        return _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.DoctorSchedule)
            .Include(a => a.Prescriptions)
                .ThenInclude(p => p.Medication)
            .Where(a => a.DoctorSchedule.DoctorId == doctorId)
            .OrderByDescending(a => a.DoctorSchedule.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Appointment>> GetConfirmedAppointmentsAsync(string doctorId)
    {
        return _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.DoctorSchedule)
            .Where(a => a.DoctorSchedule.DoctorId == doctorId &&
                        a.Status == Enums.AppointmentStatus.Confirmed)
            .OrderByDescending(a => a.DoctorSchedule.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Medication>> GetMedicationsAsync()
    {
        return _context.Medications.OrderBy(m => m.Name).AsNoTracking().ToListAsync();
    }

    public async Task<bool> AddPrescriptionAsync(Prescription prescription, string doctorId, string appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.DoctorSchedule)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorSchedule.DoctorId == doctorId);
        if (appointment is null)
        {
            return false;
        }

        prescription.AppointmentId = appointmentId;
        _context.Prescriptions.Add(prescription);
        appointment.Status = Enums.AppointmentStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> HasOverlappingScheduleAsync(string doctorId, DateTime startTime, DateTime endTime)
    {
        return _context.DoctorSchedules
            .AnyAsync(s => s.DoctorId == doctorId &&
                           s.StartTime < endTime &&
                           s.EndTime > startTime);
    }

    public async Task<bool> ConfirmAppointmentAsync(string doctorId, string appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.DoctorSchedule)
            .FirstOrDefaultAsync(a => a.Id == appointmentId &&
                                      a.DoctorSchedule.DoctorId == doctorId &&
                                      a.Status == Enums.AppointmentStatus.Pending);
        if (appointment is null)
        {
            return false;
        }

        appointment.Status = Enums.AppointmentStatus.Confirmed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAppointmentAsync(string doctorId, string appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.DoctorSchedule)
            .FirstOrDefaultAsync(a => a.Id == appointmentId &&
                                      a.DoctorSchedule.DoctorId == doctorId &&
                                      a.Status == Enums.AppointmentStatus.Pending);
        if (appointment is null)
        {
            return false;
        }

        appointment.Status = Enums.AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    // Aggregate queries — executed at DB level, return only counts
    public Task<int> GetAppointmentsCountAsync(string doctorId)
    {
        return _context.Appointments
            .CountAsync(a => a.DoctorSchedule.DoctorId == doctorId);
    }

    public Task<int> GetDistinctPatientsCountAsync(string doctorId)
    {
        return _context.Appointments
            .Where(a => a.DoctorSchedule.DoctorId == doctorId)
            .Select(a => a.PatientId)
            .Distinct()
            .CountAsync();
    }

    public Task<int> GetPendingAppointmentsCountAsync(string doctorId)
    {
        return _context.Appointments
            .CountAsync(a => a.DoctorSchedule.DoctorId == doctorId &&
                             a.Status == Enums.AppointmentStatus.Pending);
    }
}
