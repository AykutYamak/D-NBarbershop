using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Services;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Appointments
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public Guid BarberId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan AppointmentTime { get; set; }
        [Required]
        public AppointmentStatus Status { get; set; }

        public List<Service> Services { get; set; } = new ();
        public List<Guid> SelectedServiceIds { get; set; } = new ();
        public List<Barber> Barbers { get; set; } = new();
    }
}
