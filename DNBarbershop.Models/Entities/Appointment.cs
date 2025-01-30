using DNBarbershop.Models.EnumClasses;
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
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Barber))]
        public Guid BarberId { get; set; }
        public Barber Barber { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Service))]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime AppointmentDate { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public TimeSpan AppointmentTime { get; set; }
        [Required]
        public AppointmentStatus Status { get; set; }

        public ICollection<AppointmentServices> AppointmentServices { get; set; } = new HashSet<AppointmentServices>();

    }
}
