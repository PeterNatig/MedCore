using System.ComponentModel.DataAnnotations;

namespace ViewModel
{
    public class DoctorRegisterVM
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
        public MedCore.Model.Enums.Gender Gender { get; set; }
        [Display(Name = "Specialty")]
        public string? SpecialtyId { get; set; }
        [Display(Name = "License Number")]
        [Required]
        public string LicenseNumber { get; set; }
        [Display(Name = "Years of Experience")]
        [Required]
        public int ExperienceYears { get; set; }
        [Required]
        [Display(Name = "Hourly Rate")]
        public decimal HourRate { get; set; }
        public string? Bio { get; set; }
        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
