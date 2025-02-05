using DNBarbershop.Models.Entities;

namespace DNBarbershop.Models.ViewModels.Services
{
    public class ServiceViewModel
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsSelected { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<AppointmentServices> AppointmentServices { get; set; } = new HashSet<AppointmentServices>();
    }
}
