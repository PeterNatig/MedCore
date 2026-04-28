using MedCore.Model;

namespace Repository
{
    public interface IDoctorRepo : IUserRepo<Doctor>
    {
        Task<bool> Login(Doctor doc, string password, bool remeber);
        Doctor GetDoctorByEmail(string email);
        Task<List<Specialty>> GetSpecialtiesAsync();
    }
}
