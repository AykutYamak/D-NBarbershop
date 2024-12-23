using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class WorkSchedule
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Barber))]
        public Guid BarberId { get; set; }
        public Barber Barber { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime WorkDate { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        [NotMapped]
        public TimeSpan WorkTimeDuration => EndTime - StartTime;

    }
}
