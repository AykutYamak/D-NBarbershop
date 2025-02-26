using System.ComponentModel.DataAnnotations;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.ViewModels.Users
{
    public class UserEditViewModel
    {

        public string Id { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string FirstName { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
