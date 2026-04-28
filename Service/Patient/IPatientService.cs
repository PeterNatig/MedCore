using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using ViewModel;

namespace Service
{
    public interface IPatientService
    {
        Task<IdentityResult> Add(PatientRegisterVM pat);
        Task<bool> Login(LoginVM pat);
        Patient GetByEmail(string Email);
        int Save();

    }
}

