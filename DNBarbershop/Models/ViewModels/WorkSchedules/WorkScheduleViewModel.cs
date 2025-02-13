using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.WorkSchedules
{
    public class WorkScheduleViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BarberId { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan WorkTimeDuration => EndTime - StartTime;
        public IEnumerable<WorkSchedule> WorkSchedules { get; set; }
        public SelectList Barbers { get; set; }




    }
}
