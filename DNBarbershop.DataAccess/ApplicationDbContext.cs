using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Appointment> appointments { get; set; }    
        public DbSet<Barber> barbers { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Feedback> feedbacks { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<Speciality> speciality { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<WorkSchedule> workSchedules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
           
        }
    }
}
