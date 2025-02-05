using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Specialities
{
    public class SpecialityEditViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(20, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Type { get; set; }

        public ICollection<Barber> Barbers { get; set; } = new HashSet<Barber>();
    }
}
