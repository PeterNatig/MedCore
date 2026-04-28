using Microsoft.AspNetCore.Identity;
using static MedCore.Model.Enums;

namespace MedCore.Model
{
    public abstract class User : IdentityUser
    {
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] Version { get; set; }
    }
}
