using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.Appointments
{
    public class AppointmentFilterViewModel
    {
        public string? UserId { get; set; }
        public Guid? BarberId { get; set; }
        public DateTime AppointmentDates { get; set; }
        public SelectList? Barbers { get; set; }
        public SelectList? Users { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}
