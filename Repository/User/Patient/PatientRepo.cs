using MedCore.Data;
using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PatientRepo : UserRepo<Patient>, IPatientRepo
    {
        readonly MedCoreDbContext _context;
        readonly SignInManager<User> _signIn;

        public PatientRepo(UserManager<User> userManager, MedCoreDbContext db, SignInManager<User> signInManager)
            : base(userManager, db)
        {
            _signIn = signInManager;
            _context = db;
        }

        public async Task<bool> Login(Patient patient, string password, bool remember)
        {
            var result = await _signIn.PasswordSignInAsync(patient, password, remember, false);
            return result.Succeeded;
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Email == email);
        }

        // Keep sync version for backward compat with IPatientRepo
        public Patient GetPatientByEmail(string email)
        {
            return _context.Patients.FirstOrDefault(e => e.Email == email);
        }
    }
}
