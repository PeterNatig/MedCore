using Repository.Workflow;
using ViewModel.Patient;
using ViewModel.Shared;

namespace Service.Workflow;

public class PatientWorkflowService : IPatientWorkflowService
{
    private readonly IPatientWorkflowRepo _repo;

    public PatientWorkflowService(IPatientWorkflowRepo repo)
    {
        _repo = repo;
    }

    public async Task<PatientPortalHomeVM> GetSpecialtiesAsync()
    {
        var specialties = await _repo.GetSpecialtiesAsync();
        return new PatientPortalHomeVM
        {
            Specialties = specialties
                .Select(s => new SpecialtyCardVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DoctorsCount = s.Doctors?.Count ?? 0
                })
                .ToList()
        };
    }

    public async Task<DoctorsBySpecialtyVM> GetDoctorsBySpecialtyAsync(string specialtyId)
    {
        var doctors = await _repo.GetDoctorsBySpecialtyAsync(specialtyId);
        var specialtyName = doctors.FirstOrDefault()?.Specialty?.Name ?? "Specialty";
        return new DoctorsBySpecialtyVM
        {
            SpecialtyId = specialtyId,
            SpecialtyName = specialtyName,
            Doctors = doctors.Select(d => new DoctorCardVM
            {
                Id = d.Id,
                FullName = d.FullName,
                SpecialtyName = d.Specialty?.Name ?? string.Empty,
                ExperienceYears = d.ExperienceYears ?? 0,
                HourRate = d.HourRate ?? 0,
                Bio = d.Bio
            }).ToList()
        };
    }

    public async Task<BookAppointmentVM?> GetBookingModelAsync(string doctorId, string patientEmail)
    {
        var patient = await _repo.GetPatientByEmailAsync(patientEmail);
        if (patient is null)
        {
            return null;
        }

        var doctor = await _repo.GetDoctorWithAvailableSlotsAsync(doctorId, patient.Id);
        if (doctor is null)
        {
            return null;
        }

        return new BookAppointmentVM
        {
            DoctorId = doctor.Id,
            DoctorName = doctor.FullName,
            SpecialtyName = doctor.Specialty?.Name ?? string.Empty,
            AvailableSlots = doctor.Schedules?.Select(s => new AvailableSlotVM
            {
                ScheduleId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsBooked = s.IsBooked
            }).ToList() ?? []
        };
    }

    public async Task<(bool Success, string Message)> BookAppointmentAsync(string patientEmail, string scheduleId)
    {
        var patient = await _repo.GetPatientByEmailAsync(patientEmail);
        if (patient is null)
        {
            return (false, "Patient account not found.");
        }

        return await _repo.BookAppointmentAsync(patient.Id, scheduleId);
    }

    public async Task<MyRecordsVM> GetRecordsAsync(string patientEmail)
    {
        var patient = await _repo.GetPatientByEmailAsync(patientEmail);
        if (patient is null)
        {
            return new MyRecordsVM();
        }

        var appointments = await _repo.GetPatientAppointmentsAsync(patient.Id);
        return new MyRecordsVM
        {
            Records = appointments.Select(a => new MyRecordItemVM
            {
                AppointmentId = a.Id,
                DoctorName = a.DoctorSchedule.Doctor.FullName,
                SpecialtyName = a.DoctorSchedule.Doctor.Specialty?.Name ?? string.Empty,
                AppointmentDate = a.DoctorSchedule.StartTime,
                Status = a.Status.ToString(),
                Prescriptions = a.Prescriptions?.Select(p => new PrescriptionItemVM
                {
                    MedicationName = p.Medication?.Name ?? "Unknown",
                    Dosage = p.Dosage,
                    Frequency = p.Frequency
                }).ToList() ?? []
            }).ToList()
        };
    }

    public async Task<PatientPrescriptionsVM> GetPrescriptionsAsync(string patientEmail)
    {
        var patient = await _repo.GetPatientByEmailAsync(patientEmail);
        if (patient is null)
        {
            return new PatientPrescriptionsVM();
        }

        var appointments = await _repo.GetPatientAppointmentsAsync(patient.Id);
        var prescriptions = appointments
            .Where(a => a.Prescriptions != null && a.Prescriptions.Any())
            .SelectMany(a => a.Prescriptions.Select(p => new PrescriptionDetailVM
            {
                PrescriptionId = p.Id,
                MedicationName = p.Medication?.Name ?? "Unknown",
                GenericName = p.Medication?.GenericName ?? string.Empty,
                Dosage = p.Dosage,
                Frequency = p.Frequency,
                DoctorName = a.DoctorSchedule.Doctor.FullName,
                SpecialtyName = a.DoctorSchedule.Doctor.Specialty?.Name ?? string.Empty,
                AppointmentDate = a.DoctorSchedule.StartTime,
                PrescribedDate = p.LastModified
            }))
            .OrderByDescending(p => p.PrescribedDate)
            .ToList();

        return new PatientPrescriptionsVM { Prescriptions = prescriptions };
    }

    public async Task<bool> CancelAppointmentAsync(string patientEmail, string appointmentId)
    {
        var patient = await _repo.GetPatientByEmailAsync(patientEmail);
        if (patient is null)
        {
            return false;
        }

        return await _repo.CancelAppointmentAsync(patient.Id, appointmentId);
    }
}
