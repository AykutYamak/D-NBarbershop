using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Barbers
{
    public class BarberEditViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string FirstName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public List<SelectListItem> Specialities { get; set; } = new();
        public Guid SelectedSpecialityId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int ExperienceYears { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string ProfilePictureUrl { get; set; }
        public IFormFile? imageFile { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
