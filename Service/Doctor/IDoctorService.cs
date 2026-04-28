using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModel;

namespace Service
{
    public interface IDoctorService
    {
        Task<IdentityResult> Add(DoctorRegisterVM doc);
        int Save();
        Task<bool> Login(LoginVM doc);
        Doctor GetByEmail(string Email);
        Task<List<SelectListItem>> GetSpecialtySelectListAsync();
    }
}
