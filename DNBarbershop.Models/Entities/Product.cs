using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(255,ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Name { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(1000,ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Description { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public decimal Price { get; set; }
        [Required(ErrorMessage =RequiredErrorMessage)]
        [Range(0,int.MaxValue,ErrorMessage = NonNegativeNumberErrorMessage)]
        [RegularExpression(@"^\d+$", ErrorMessage = MustBeWholeNumberErrorMessage)]
        public int StockQuantity { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderDetail> orderDetails { get; set; } = new HashSet<OrderDetail>();

    }
}
