using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class AppointmentService
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        [Required]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
