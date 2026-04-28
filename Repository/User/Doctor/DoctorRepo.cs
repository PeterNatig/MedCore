using MedCore.Data;
using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class DoctorRepo : UserRepo<Doctor>, IDoctorRepo
    {
        private readonly SignInManager<User> _signIn;
        private readonly MedCoreDbContext _context;

        public DoctorRepo(UserManager<User> userManager, MedCoreDbContext db, SignInManager<User> signInManager)
            : base(userManager, db)
        {
            _signIn = signInManager;
            _context = db;
        }

        public Doctor GetDoctorByEmail(string email)
        {
            return _context.Doctors.FirstOrDefault(e => e.Email == email);
        }

        public async Task<bool> Login(Doctor doc, string password, bool remember)
        {
            var result = await _signIn.PasswordSignInAsync(doc, password, remember, false);
            return result.Succeeded;
        }

        public Task<List<Specialty>> GetSpecialtiesAsync()
        {
            return _context.Specialties.OrderBy(s => s.Name).AsNoTracking().ToListAsync();
        }
    }
}
