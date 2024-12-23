using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage=RequiredErrorMessage)]
        [StringLength(50,ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string Name { get; set; }
        [Required(ErrorMessage=RequiredErrorMessage)]
        [StringLength(500,ErrorMessage =nameof(MaxLengthExceededErrorMessage))]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}