using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Models.ViewModels.Services
{
    public class ServiceFilterViewModel
    {
        public decimal? MaxPrice{ get; set; }
        public List<Service> Services { get; set; }
    }
}
