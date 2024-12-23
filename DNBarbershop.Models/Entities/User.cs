﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DNBarbershop.Common.ErrorMessages;
namespace DNBarbershop.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string FirstName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string LastName { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailErrorMessage)]
        public string Email { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = nameof(MinimumLengthErrorMessage))]
        public string Password { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(15, ErrorMessage = nameof(MaxLengthExceededErrorMessage))]
        public string PhoneNumber { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
    }
}