namespace DNBarbershop.Models.ViewModels.Barbers
{
    public class SingleBarberViewModel
    {
        public Guid id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Speciality { get; set; }
        public int ExperienceYears { get; set; }
        public string Description { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
