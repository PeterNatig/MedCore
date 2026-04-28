using Microsoft.AspNetCore.Identity;

namespace Repository
{
    public interface IUserRepo<T>
    {
        Task<IdentityResult> Add(T model, string password, string role);
        int Save();
    }
}
