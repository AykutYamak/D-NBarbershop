using DNBarbershop.Models.Entities;

namespace DNBarbershop.Models.ViewModels.Users
{
    public class UserViewModel
    {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
