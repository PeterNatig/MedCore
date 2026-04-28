using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository;
using ViewModel;

namespace Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepo _doctorRepo;

        public DoctorService(IDoctorRepo doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        public async Task<IdentityResult> Add(DoctorRegisterVM doc)
        {
            var doctor = new Doctor
            {
                Bio = doc.Bio,
                DateOfBirth = doc.BirthDate,
                HourRate = doc.HourRate,
                LicenseNumber = doc.LicenseNumber,
                FullName = doc.FullName,
                UserName = doc.UserName,
                Email = doc.Email,
                NationalId = doc.NationalID,
                Gender = doc.Gender,
                ExperienceYears = doc.ExperienceYears,
                SpecialtyId = doc.SpecialtyId
            };
            return await _doctorRepo.Add(doctor, doc.Password, "Doctor");
        }

        public Doctor GetByEmail(string email)
        {
            return _doctorRepo.GetDoctorByEmail(email);
        }

        public async Task<bool> Login(LoginVM doc)
        {
            var doctor = GetByEmail(doc.Email);
            if (doctor is null)
            {
                return false;
            }
            return await _doctorRepo.Login(doctor, doc.Password, doc.RememberMe);
        }

        public int Save()
        {
            return _doctorRepo.Save();
        }

        public async Task<List<SelectListItem>> GetSpecialtySelectListAsync()
        {
            var specialties = await _doctorRepo.GetSpecialtiesAsync();
            return specialties.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = s.Name
            }).ToList();
        }
    }
}
