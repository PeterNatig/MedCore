using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Repository;
using ViewModel;

namespace Service
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepo _patientRepo;

        public PatientService(IPatientRepo patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<IdentityResult> Add(PatientRegisterVM pat)
        {
            var patient = new Patient
            {
                FullName = pat.FullName,
                UserName = pat.UserName,
                DateOfBirth = pat.BirthDate,
                Gender = pat.Gender,
                BloodType = pat.BloodType,
                Email = pat.Email,
                NationalId = pat.NationalID
            };
            patient.Allergies.Details = pat.Allergies;
            patient.ChronicConditions.Details = pat.Chronic;

            return await _patientRepo.Add(patient, pat.Password, "Patient");
        }

        public Patient GetByEmail(string email)
        {
            return _patientRepo.GetPatientByEmail(email);
        }

        public async Task<bool> Login(LoginVM pat)
        {
            var patient = GetByEmail(pat.Email);
            if (patient is null)
            {
                return false;
            }
            return await _patientRepo.Login(patient, pat.Password, pat.RememberMe);
        }

        public int Save()
        {
            return _patientRepo.Save();
        }
    }
}
