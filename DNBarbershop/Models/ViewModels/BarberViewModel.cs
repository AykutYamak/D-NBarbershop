using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels
{
    public class BarberViewModel
    {
        public Guid? SpecialityId { get; set; }
        public int? MinExperienceYears { get; set; }

        public SelectList Specialities { get; set; }
        public List<Barber> Barbers { get; set; }
    }
}
