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
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage=RequiredErrorMessage)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Barber))]
        public Guid? BarberId { get; set; }
        public Barber? Barber { get; set; }
        [ForeignKey(nameof(Service))]
        public Guid? ServiceId { get; set; }
        public Service Service { get; set; }
        [Required(ErrorMessage=RequiredErrorMessage)]
        [Range(0,5, ErrorMessage = "Rating should be in the range [0;5]")]
        public int Rating { get; set; }
        [StringLength(1000,ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Comment { get; set; }
        [Required(ErrorMessage=RequiredErrorMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FeedBackDate { get; set; }

    }
}
