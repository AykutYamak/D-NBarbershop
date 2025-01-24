using DNBarbershop.Common;
using DNBarbershop.Models.EnumClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(0,100,ErrorMessage = NonNegativeNumberErrorMessage)]
        public decimal TotalAmount { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        public ICollection<OrderDetail> orderDetails { get; set; } = new HashSet<OrderDetail>();
    } 
}
