using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.Messages
{
    public class MessageFilterViewModel
    {
        public string? UserId { get; set; }
        public SelectList Users { get; set; }
        public List<Message> Messages { get; set; }
    }
}
