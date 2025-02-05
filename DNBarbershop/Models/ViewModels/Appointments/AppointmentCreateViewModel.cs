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

        [Required(ErrorMessage = RequiredErrorMessage)]
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public Guid BarberId { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan AppointmentTime { get; set; }
        [Required]
        public AppointmentStatus Status { get; set; }

        public List<ServiceViewModel> Services { get; set; } = new List<ServiceViewModel>();
        public List<Guid> SelectedServiceIds { get; set; } = new List<Guid>();

    }
}
