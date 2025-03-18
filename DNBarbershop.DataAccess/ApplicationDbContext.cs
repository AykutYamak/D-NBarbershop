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
        public DbSet<Feedback> feedbacks { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<Speciality> speciality { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Message> messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
           .HasOne(f => f.User)
           .WithMany(u => u.Messages)
           .HasForeignKey(f => f.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Barber)
            .WithMany(b => b.Feedbacks)
            .HasForeignKey(f => f.BarberId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppointmentServices>()
            .HasKey(a => new { a.AppointmentId, a.ServiceId });

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Appointment)
            .WithMany(a => a.AppointmentServices)
            .HasForeignKey(a => a.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppointmentServices>()
            .HasOne(a => a.Service)
            .WithMany(s => s.AppointmentServices)
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Speciality>()
                .HasMany(s => s.Barbers)
                .WithOne(b => b.Speciality)
                .HasForeignKey(b => b.SpecialityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Speciality>().HasData
                (
                    new Speciality
                    {
                        Type = "Стажант"
                    },
                    new Speciality
                    {
                        Type = "Шеф"
                    },
                    new Speciality
                    {
                        Type="Младши"
                    }
                );

            modelBuilder.Entity<Service>().HasData
                (
                    new Service
                    {
                        ServiceName="Фейд прическа",
                        Description = "Стилна и модерна подстрижка, която придава изчистен и свеж вид. Фейдът се характеризира с плавно преливане на дължините, започвайки от късо или дори нула в долната част, постепенно преминавайки към по-дълга коса отгоре. Подходяща е за всякакви типове коса и може да бъде персонализирана според предпочитанията на клиента – нисък, среден или висок фейд.",
                        Price = 15,
                        Duration = new TimeSpan(0,30,0)
                    },
                    new Service
                    {
                        ServiceName = "Класическа прическа",
                        Description = "Вечен стил, който подчертава елегантността и поддържания външен вид. Тази подстрижка се характеризира с равномерна дължина и чисти линии, без резки преходи. Подходяща е както за ежедневието, така и за официални събития. Може да се стилизира по различни начини – с път, загладена назад или с лек обем, в зависимост от предпочитанията на клиента.",
                        Price= 10,
                        Duration = new TimeSpan(0, 30, 0)
                    },
                    new Service
                    {
                        ServiceName = "Оформяне и подстригване на брада",
                        Description = "За добре поддържан и стилен вид. Тази услуга включва оформяне на контурите, скъсяване и изравняване на дължината, както и детайлна обработка с машинка, ножица или бръснач за перфектен завършек. По желание може да се приложи гореща кърпа за по-комфортно бръснене и омекотяване на кожата.",
                        Price = 10,
                        Duration = new TimeSpan(0, 30, 0)
                    },
                    new Service
                    {
                        ServiceName = "Детска прическа",
                        Description = "Специално оформена подстрижка за малките господа, която съчетава удобство и стил. Подстригването се извършва с внимание към детайлите, като се вземат предвид желанията на родителите и комфорта на детето. Включва класически или модерни стилове, преливащи прически (фейд), изравняване и оформяне според предпочитанията.",
                        Price = 8,
                        Duration = new TimeSpan(0, 30, 0)
                    },
                    new Service
                    {
                        ServiceName = "Кралско бръснене с кърпа",
                        Description = "Луксозна услуга за перфектно гладко бръснене и релаксация. Включва подготовка на кожата с топла кърпа, омекотяване на брадата, бръснене с прав бръснач за максимална прецизност и завършек с охлаждаща кърпа и хидратиращ продукт. Осигурява свежест, комфорт и безупречен външен вид.",
                        Price = 12,
                        Duration = new TimeSpan(0, 30, 0)
                    },
                    new Service
                    {
                        ServiceName = "Боядисване",
                        Description = "Лесен и естествен начин за прикриване на побелелите коси. Процедурата използва специална боя, която се слива с естествения цвят на косата и брадата, без резки промени. Резултатът е освежен и поддържан външен вид.",
                        Price = 8,
                        Duration = new TimeSpan(0, 30, 0)
                    }
                );

            modelBuilder.Entity<Barber>().HasData
                (
                    new Barber
                    {
                        FirstName = "Иван",
                        LastName = "Георгиев",
                        SpecialityId = speciality.First(s=>s.Type == "Шеф").Id,
                        ExperienceYears = 6,
                        ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-173-scaled.jpg"
                    },
                    new Barber
                    {
                        FirstName = "Петър",
                        LastName = "Христов",
                        SpecialityId = speciality.First(s => s.Type == "Стажант").Id,
                        ExperienceYears = 0,
                        ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-123-scaled.jpg",
                    },
                    new Barber
                    {
                        FirstName = "Калоян",
                        LastName = "Георгиев",
                        SpecialityId = speciality.First(s => s.Type == "Младши").Id,
                        ExperienceYears = 1,
                        ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-132-scaled.jpg"
                    },
                    new Barber
                    {
                        FirstName = "Христо",
                        LastName = "Петров",
                        SpecialityId = speciality.First(s => s.Type == "Шеф").Id,
                        ExperienceYears = 7,
                        ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-158-1-scaled.jpg"
                    }
                );

            modelBuilder.Entity<Feedback>().HasData
                (
                    new Feedback
                    {
                        UserId = users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                        BarberId = barbers.First(b => b.FirstName == "Иван" && b.LastName == "Георгиев").Id,
                        Rating = 5,
                        Comment = "Прекрасно обслужване и страхотен резултат! Препоръчвам на всички!"
                    },
                    new Feedback
                    {
                        UserId = users.First(u => u.FirstName == "Aykut" && u.LastName == "Yamak").Id,
                        BarberId = barbers.First(b => b.FirstName == "Петър" && b.LastName == "Христов").Id,
                        Rating = 4,
                        Comment = "Много добро обслужване, но може би малко повече внимание към детайлите."
                    },
                    new Feedback
                    {
                        UserId = users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                        BarberId = barbers.First(b => b.FirstName == "Калоян" && b.LastName == "Георгиев").Id,
                        Rating = 3,
                        Comment = "Добро обслужване, но не бях напълно удовлетворен."
                    },
                    new Feedback
                    {
                        UserId = users.First(u => u.FirstName == "Aykut" && u.LastName == "Yamak").Id,
                        BarberId = barbers.First(b => b.FirstName == "Христо" && b.LastName == "Петров").Id,
                        Rating = 5,
                        Comment = "Невероятно обслужване и страхотен персонал! Препоръчвам на всички!"
                    }
                );

            modelBuilder.Entity<Message>().HasData
                (
                    new Message
                    {
                        Email = "gosho@abv.bg",
                        Content = "Не ми хареса прическата, която Иван Георгиев ми направи.",
                        Date = new DateTime(2024, 6, 15),
                        IsRead = false
                    },
                    new Message
                    {
                        Email = "teodortm@gmail.com",
                        Content = "Много съм доволен от обслужването на Петър Христов.",
                        Date = new DateTime(2024, 6, 16),
                        IsRead = false
                    }
                );

            modelBuilder.Entity<AppointmentServices>().HasData
                (
                    new AppointmentServices
                    {
                        AppointmentId = appointments.First(a => a.User.FirstName == "Иван" && a.User.LastName == "Иванов").Id,
                        ServiceId = services.First(s => s.ServiceName == "Фейд прическа").Id
                    },
                    new AppointmentServices
                    {
                        AppointmentId = appointments.First(a => a.User.FirstName == "Иван" && a.User.LastName == "Иванов").Id,
                        ServiceId = services.First(s => s.ServiceName == "Оформяне и подстригване на брада").Id
                    },
                    new AppointmentServices
                    {
                        AppointmentId = appointments.First(a => a.User.FirstName == "Aykut" && a.User.LastName == "Yamak").Id,
                        ServiceId = services.First(s => s.ServiceName == "Класическа прическа").Id
                    }
                );

            modelBuilder.Entity<Appointment>().HasData
                (
                    new Appointment
                    {
                        UserId = users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                        BarberId = barbers.First(b => b.FirstName == "Иван" && b.LastName == "Георгиев").Id,
                        AppointmentDate = new DateTime(2024, 6, 15),
                        AppointmentTime = new TimeSpan(14, 00, 00),
                        Status = DNBarbershop.Models.EnumClasses.AppointmentStatus.Completed

                    }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
