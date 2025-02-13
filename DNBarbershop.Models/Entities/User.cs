using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
        public class User : IdentityUser    
        {
            [Required(ErrorMessage = RequiredErrorMessage)]
            [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
            public string FirstName { get; set; }
            [Required(ErrorMessage = RequiredErrorMessage)]
            [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
            public string LastName { get; set; }

            public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
            public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
            public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        }
}
