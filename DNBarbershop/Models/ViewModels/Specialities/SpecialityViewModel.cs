using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace DNBarbershop.Models.ViewModels.Specialities
{
    public class SpecialityViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public ICollection<Barber> Barbers { get; set; } = new HashSet<Barber>();
    }
}
