using DNBarbershop.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Messages
{
    public class MessageCreateViewModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string Email { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string Content { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public bool IsRead { get; set; } = false;
    }
}
