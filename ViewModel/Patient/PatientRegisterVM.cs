using System.ComponentModel.DataAnnotations;
using static MedCore.Model.Enums;

namespace ViewModel
{
    public class PatientRegisterVM
    {
        [Required]
        [MinLength(14, ErrorMessage = "National ID must be 14 digits")]
        [MaxLength(14, ErrorMessage = "National ID must be 14 digits")]
        [RegularExpression("^\\d+$", ErrorMessage = "National Id must be digits")]
        [Display(Name = "National ID")]
        public string NationalID { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }
        [Display(Name = "Blood Type")]
        [Required]
        public BloodType BloodType { get; set; }

        public string? Allergies { get; set; }
        [Display(Name = "Chronic Conditions")]
        public string? Chronic { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
