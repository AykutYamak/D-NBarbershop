using DNBarbershop.Models.EnumClasses;

namespace DNBarbershop.Models.ViewModels.Appointments
{
    public class AppointmentViewModel
    {

        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public Guid BarberId { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
