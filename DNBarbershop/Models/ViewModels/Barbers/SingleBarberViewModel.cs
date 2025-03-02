using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.Barbers
{
    public class SingleBarberViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Speciality { get; set; }
        public int ExperienceYears { get; set; }
        public string ProfilePictureUrl { get; set; }

        public Guid? SpecialityId { get; set; }
        public int? MinExperienceYears { get; set; }
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
