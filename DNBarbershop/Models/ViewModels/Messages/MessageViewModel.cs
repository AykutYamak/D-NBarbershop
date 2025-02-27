using DNBarbershop.Models.Entities;

namespace DNBarbershop.Models.ViewModels.Messages
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRead { get; set; }

    }
}
