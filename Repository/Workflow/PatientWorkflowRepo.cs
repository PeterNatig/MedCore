using MedCore.Data;
using MedCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Repository.Workflow;

public class PatientWorkflowRepo : IPatientWorkflowRepo
{
    private readonly MedCoreDbContext _context;

    public PatientWorkflowRepo(MedCoreDbContext context)
    {
        _context = context;
    }

    public Task<List<Specialty>> GetSpecialtiesAsync()
    {
        return _context.Specialties
            .Include(s => s.Doctors!)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Doctor>> GetDoctorsBySpecialtyAsync(string specialtyId)
    {
        return _context.Doctors
            .Where(d => d.SpecialtyId == specialtyId)
            .Include(d => d.Specialty)
            .OrderBy(d => d.FullName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Doctor?> GetDoctorWithAvailableSlotsAsync(string doctorId, string patientId)
    {
        var now = DateTime.UtcNow;

        // Slots this patient previously cancelled — block only for THIS patient
        var patientCancelledSlots = await _context.Appointments
            .Where(a => a.PatientId == patientId && a.Status == Enums.AppointmentStatus.Cancelled)
            .Select(a => a.ScheduleId)
            .ToListAsync();

        var doctor = await _context.Doctors
            .Include(d => d.Specialty)
            .Include(d => d.Schedules!.Where(s =>
                s.StartTime >= now &&
                !patientCancelledSlots.Contains(s.Id)
            ).OrderBy(s => s.StartTime))
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        return doctor;
    }

    public Task<Patient?> GetPatientByEmailAsync(string email)
    {
        return _context.Patients.FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<(bool Success, string Message)> BookAppointmentAsync(string patientId, string scheduleId)
    {
        var schedule = await _context.DoctorSchedules
            .Include(s => s.Doctor)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule is null)
        {
            return (false, "This time slot no longer exists.");
        }

        if (schedule.IsBooked)
        {
            return (false, "This slot is no longer available.");
        }

        // 1) Check if THIS patient previously cancelled this exact slot
        var wasCancelledByPatient = await _context.Appointments
            .AnyAsync(a => a.ScheduleId == scheduleId &&
                           a.PatientId == patientId &&
                           a.Status == Enums.AppointmentStatus.Cancelled);
        if (wasCancelledByPatient)
        {
            return (false, "You previously cancelled this appointment. Please choose a different time slot.");
        }

        // 2) Check if this patient already has an active appointment with the same doctor
        var hasActiveWithDoctor = await _context.Appointments
            .AnyAsync(a => a.PatientId == patientId &&
                           a.DoctorSchedule.DoctorId == schedule.DoctorId &&
                           (a.Status == Enums.AppointmentStatus.Pending ||
                            a.Status == Enums.AppointmentStatus.Confirmed));
        if (hasActiveWithDoctor)
        {
            return (false, "You already have an active appointment with this doctor. Cancel it first to book a new one.");
        }

        // 3) Check if this patient has an active appointment at an overlapping time with any doctor
        var hasTimeConflict = await _context.Appointments
            .AnyAsync(a => a.PatientId == patientId &&
                           (a.Status == Enums.AppointmentStatus.Pending ||
                            a.Status == Enums.AppointmentStatus.Confirmed) &&
                           a.DoctorSchedule.StartTime < schedule.EndTime &&
                           a.DoctorSchedule.EndTime > schedule.StartTime);
        if (hasTimeConflict)
        {
            return (false, "You already have an appointment at this time. Cancel it first or choose a different slot.");
        }

        // Remove any existing cancelled appointment from a different patient
        // (one-to-one constraint: only one appointment per schedule)
        var existingCancelled = await _context.Appointments
            .FirstOrDefaultAsync(a => a.ScheduleId == scheduleId &&
                                      a.Status == Enums.AppointmentStatus.Cancelled);
        if (existingCancelled is not null)
        {
            _context.Appointments.Remove(existingCancelled);
        }

        schedule.IsBooked = true;
        _context.Appointments.Add(new Appointment
        {
            PatientId = patientId,
            ScheduleId = scheduleId,
            Status = Enums.AppointmentStatus.Pending,
            CancellationReason = string.Empty
        });

        await _context.SaveChangesAsync();
        return (true, "Appointment booked successfully! Waiting for doctor confirmation.");
    }

    public Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId)
    {
        return _context.Appointments
            .Where(a => a.PatientId == patientId)
            .Include(a => a.DoctorSchedule)
                .ThenInclude(s => s.Doctor)
                    .ThenInclude(d => d.Specialty)
            .Include(a => a.Prescriptions)
                .ThenInclude(p => p.Medication)
            .OrderByDescending(a => a.DoctorSchedule.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> CancelAppointmentAsync(string patientId, string appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.DoctorSchedule)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId);

        if (appointment is null ||
            (appointment.Status != Enums.AppointmentStatus.Pending &&
             appointment.Status != Enums.AppointmentStatus.Confirmed))
        {
            return false;
        }

        appointment.Status = Enums.AppointmentStatus.Cancelled;
        appointment.CancellationReason = "Cancelled by patient";
        appointment.DoctorSchedule.IsBooked = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
