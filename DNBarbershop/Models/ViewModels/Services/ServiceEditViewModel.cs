using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Services
{
    public class ServiceEditViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(50, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string ServiceName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(500, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Description { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public decimal Price { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan Duration { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<AppointmentServices> AppointmentServices { get; set; } = new HashSet<AppointmentServices>();
    }
}
