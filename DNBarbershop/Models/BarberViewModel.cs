using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;

namespace DNBarbershop.Models
{
    public class BarberViewModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string FirstName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Speciality))]
        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public int ExperienceYears { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string ProfilePictureUrl { get; set; }
    }
}
