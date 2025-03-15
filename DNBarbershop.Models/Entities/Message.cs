using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Невалиден e-mail формат.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(\.[a-zA-Z]{2,})?$",
        ErrorMessage = "Вашият e-mail трябва да съдържа валиден домейн и поне една точка, както и символ '@'!")]
        public string Email { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string Content { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public User? User { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public bool IsRead { get; set; } = false;
    }
}
