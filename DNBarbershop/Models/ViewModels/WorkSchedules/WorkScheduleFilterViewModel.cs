using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.WorkSchedules
{
    public class WorkScheduleFilterViewModel
    {

        public Guid? BarberId { get; set; }
        public SelectList? Barbers { get; set; }
        public List<WorkSchedule>? WorkSchedules { get; set; }
    }
}
