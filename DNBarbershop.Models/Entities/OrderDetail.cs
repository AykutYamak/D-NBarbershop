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
    public class OrderDetail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        [Required(ErrorMessage =RequiredErrorMessage)]
        [Range(0, int.MaxValue, ErrorMessage = NonNegativeNumberErrorMessage)]
        [RegularExpression(@"^\d+$", ErrorMessage = MustBeWholeNumberErrorMessage)]
        public int Quantity { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public decimal Price { get; set; }


    }
}
