using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.WorkSchedules
{
    public class WorkScheduleEditViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Barber))]
        public List<SelectListItem> Barbers { get; set; } = new();
        public Guid BarberId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime WorkDate { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan EndTime { get; set; }

        [NotMapped]
        public TimeSpan WorkTimeDuration => EndTime - StartTime;
    }
}
