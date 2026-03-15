using System;
using System.Collections.Generic;
using System.Text;
using static MedCore.Model.Enums;

namespace MedCore.Model
{
    public abstract class User : BaseEntity
    {
        public string FullName { get; set; }
        public string NationalId { get; set; }      
        public DateTime DateOfBirth { get; set; }   
        public Gender Gender { get; set; }
        public byte[] ProfileImage { get; set; }
    }
}
