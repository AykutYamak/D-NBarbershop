using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DNBarbershop.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Appointment> appointments { get; set; }    
        public DbSet<AppointmentServices> appointmentServices { get; set; }
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
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Barber)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.BarberId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Barber>()
            .HasOne(b => b.Speciality)
            .WithMany(s => s.Barbers)
            .HasForeignKey(b => b.SpecialityId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Barber)
            .WithMany(b => b.Feedbacks)
            .HasForeignKey(f => f.BarberId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.orderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.orderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WorkSchedule>()
            .HasOne(ws => ws.Barber)
            .WithMany(b => b.WorkSchedules)
            .HasForeignKey(ws => ws.BarberId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AppointmentServices>()
            .HasKey(a => new { a.AppointmentId, a.ServiceId });

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Appointment)
            .WithMany(a => a.AppointmentServices)
            .HasForeignKey(a => a.AppointmentId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Service)
            .WithMany(s => s.AppointmentServices)
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Speciality>()
                .HasMany(s => s.Barbers)
                .WithOne(b => b.Speciality)
                .HasForeignKey(b => b.SpecialityId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
