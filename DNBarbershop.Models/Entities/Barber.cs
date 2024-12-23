using Microsoft.EntityFrameworkCore.Query;
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
    public class Barber
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
        [ForeignKey(nameof(Speciality))]
        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public int ExperienceYears { get; set; }
        [Required(ErrorMessage = RequiredErrorMessage)]
        public string ProfilePictureUrl { get; set; }

        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new HashSet<WorkSchedule>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
