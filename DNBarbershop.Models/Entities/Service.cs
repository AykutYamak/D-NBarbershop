using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;

namespace DNBarbershop.Models.Entities
{
    public class Service
    {
        [Key]
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
        public ICollection<AppointmentServices> AppointmentServices { get; set; } = new List<AppointmentServices>();
    }
}
