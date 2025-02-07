using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.Feedbacks
{
    public class FeedbackFilterViewModel
    {
        public Guid? BarberId { get; set; }
        public SelectList Barbers{get;set;}
        public List<Feedback> Feedbacks { get; set; }
    }
}
