using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
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
            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Barber)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.BarberId);

            modelBuilder.Entity<Barber>()
            .HasOne(b => b.Speciality)
            .WithMany(s => s.Barbers)
            .HasForeignKey(b => b.SpecialityId);

            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Barber)
            .WithMany(b => b.Feedbacks)
            .HasForeignKey(f => f.BarberId);

            modelBuilder.Entity<Feedback>()
           .HasOne(f => f.Service)
           .WithMany(s => s.Feedbacks)
           .HasForeignKey(f => f.ServiceId);

            modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.orderDetails)
            .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.orderDetails)
            .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<WorkSchedule>()
            .HasOne(ws => ws.Barber)
            .WithMany(b => b.WorkSchedules)
            .HasForeignKey(ws => ws.BarberId);

            modelBuilder.Entity<AppointmentServices>()
            .HasKey(a => new { a.AppointmentId, a.ServiceId });

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Appointment)
            .WithMany(a => a.AppointmentServices)
            .HasForeignKey(a => a.AppointmentId);

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Service)
            .WithMany(s => s.AppointmentServices)
            .HasForeignKey(a => a.ServiceId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
