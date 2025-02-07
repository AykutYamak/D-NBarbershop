using DNBarbershop.Common;
using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace DNBarbershop.Models.ViewModels.Feedbacks
{
    public class FeedbackCreateViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public List<SelectListItem> Barbers { get; set; } = new();
        public Guid SelectedBarberId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(0, 5, ErrorMessage = "Rating should be in the range [0;5]")]
        public int Rating { get; set; }
        [StringLength(1000, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Comment { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FeedBackDate { get; set; } = DateTime.Now;
    }
}
