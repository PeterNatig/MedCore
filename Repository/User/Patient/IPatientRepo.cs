using MedCore.Model;

namespace Repository
{
    public interface IPatientRepo : IUserRepo<Patient>
    {
        Task<bool> Login(Patient patient, string password, bool remember);
        Patient GetPatientByEmail(string email);
    }
}
