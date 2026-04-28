using MedCore.Data;
using MedCore.Model;
using Microsoft.AspNetCore.Identity;

namespace Repository
{
    public abstract class UserRepo<T> : IUserRepo<T> where T : User
    {
        private readonly UserManager<User> _userManager;
        private readonly MedCoreDbContext _context;

        protected UserRepo(UserManager<User> userManager, MedCoreDbContext db)
        {
            _userManager = userManager;
            _context = db;
        }

        public async Task<IdentityResult> Add(T model, string password, string role)
        {
            var result = await _userManager.CreateAsync(model, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(model, role);
            }
            return result;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
