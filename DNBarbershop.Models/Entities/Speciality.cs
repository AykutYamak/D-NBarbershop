using DNBarbershop.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class Speciality
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(20, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Type { get; set; }

        public ICollection<Barber> Barbers { get; set; } = new HashSet<Barber>();
    }
}
