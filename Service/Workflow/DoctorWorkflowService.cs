using MedCore.Model;
using Repository.Workflow;
using ViewModel.Doctor;

namespace Service.Workflow;

public class DoctorWorkflowService : IDoctorWorkflowService
{
    private readonly IDoctorWorkflowRepo _repo;

    public DoctorWorkflowService(IDoctorWorkflowRepo repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Resolves doctor by email and returns null early if not found.
    /// Centralizes the repeated pattern across all methods (DRY).
    /// </summary>
    private async Task<Doctor?> ResolveDoctorAsync(string doctorEmail)
    {
        return await _repo.GetDoctorByEmailAsync(doctorEmail);
    }

    public async Task<DoctorDashboardVM?> GetDashboardAsync(string doctorEmail)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return null;

        // Aggregate queries — run at DB level, no entity materialization
        var slotsCount = (await _repo.GetSchedulesAsync(doctor.Id)).Count;
        var appointmentsCount = await _repo.GetAppointmentsCountAsync(doctor.Id);
        var patientsCount = await _repo.GetDistinctPatientsCountAsync(doctor.Id);
        var pendingCount = await _repo.GetPendingAppointmentsCountAsync(doctor.Id);

        return new DoctorDashboardVM
        {
            DoctorName = doctor.FullName,
            SpecialtyName = doctor.Specialty?.Name,
            SlotsCount = slotsCount,
            AppointmentsCount = appointmentsCount,
            PatientsCount = patientsCount,
            PendingCount = pendingCount
        };
    }

    public async Task<DoctorScheduleVM?> GetSchedulesAsync(string doctorEmail)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return null;

        var schedules = await _repo.GetSchedulesAsync(doctor.Id);
        return new DoctorScheduleVM
        {
            Schedules = schedules.Select(s => new DoctorScheduleItemVM
            {
                ScheduleId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsBooked = s.IsBooked,
                IsPast = s.EndTime <= DateTime.Now
            }).ToList()
        };
    }

    public async Task<bool> AddScheduleAsync(string doctorEmail, DateTime startTime, DateTime endTime)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null || endTime <= startTime) return false;

        if (startTime < DateTime.UtcNow) return false;

        if (await _repo.HasOverlappingScheduleAsync(doctor.Id, startTime, endTime))
            return false;

        await _repo.AddScheduleAsync(new DoctorSchedule
        {
            DoctorId = doctor.Id,
            StartTime = startTime,
            EndTime = endTime
        });
        return true;
    }

    public async Task<bool> DeleteScheduleAsync(string doctorEmail, string scheduleId)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return false;

        return await _repo.DeleteScheduleAsync(doctor.Id, scheduleId);
    }

    public async Task<DoctorPatientsVM?> GetPatientsAsync(string doctorEmail)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return null;

        var appointments = await _repo.GetDoctorAppointmentsAsync(doctor.Id);
        return new DoctorPatientsVM
        {
            Patients = appointments.Select(a => new DoctorPatientItemVM
            {
                AppointmentId = a.Id,
                PatientName = a.Patient.FullName,
                NationalId = a.Patient.NationalId,
                AppointmentDate = a.DoctorSchedule.StartTime,
                Status = a.Status.ToString(),
                IsPast = a.DoctorSchedule.StartTime < DateTime.Now
            }).ToList()
        };
    }

    public async Task<ConsultationVM?> GetConsultationAsync(string doctorEmail)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return null;

        // Only fetch confirmed appointments — filtered at DB level
        var confirmedAppointments = await _repo.GetConfirmedAppointmentsAsync(doctor.Id);
        var medications = await _repo.GetMedicationsAsync();

        return new ConsultationVM
        {
            OpenAppointments = confirmedAppointments
                .Select(a => new DoctorPatientItemVM
                {
                    AppointmentId = a.Id,
                    PatientName = a.Patient.FullName,
                    NationalId = a.Patient.NationalId,
                    AppointmentDate = a.DoctorSchedule.StartTime,
                    Status = a.Status.ToString()
                }).ToList(),
            Medications = medications.Select(m => new MedicationOptionVM
            {
                Id = m.Id,
                Name = m.Name
            }).ToList()
        };
    }

    public async Task<bool> AddPrescriptionAsync(string doctorEmail, ConsultationVM model)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return false;

        return await _repo.AddPrescriptionAsync(new Prescription
        {
            MedicationId = model.MedicationId,
            Dosage = model.Dosage,
            Frequency = model.Frequency
        }, doctor.Id, model.AppointmentId);
    }

    public async Task<bool> ConfirmAppointmentAsync(string doctorEmail, string appointmentId)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return false;

        return await _repo.ConfirmAppointmentAsync(doctor.Id, appointmentId);
    }

    public async Task<bool> RejectAppointmentAsync(string doctorEmail, string appointmentId)
    {
        var doctor = await ResolveDoctorAsync(doctorEmail);
        if (doctor is null) return false;

        return await _repo.RejectAppointmentAsync(doctor.Id, appointmentId);
    }
}
