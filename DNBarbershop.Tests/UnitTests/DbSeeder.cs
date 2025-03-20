using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.DataAccess;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Tests.UnitTests
{
    public class DbSeeder
    {
        public static void SeedDatabase(ApplicationDbContext context)
        {

        }

        public static void SeedUsers(ApplicationDbContext context)
        {
            var admin = new User()
            {
                UserName = "adminTest",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "0898647382",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var user = new User()
            {
                UserName = "userTest",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                PhoneNumber = "0894738823",
                FirstName = "User",
                LastName = "User",
                EmailConfirmed = true
            };
            
            context.Users.Add(admin);
            context.Users.Add(user);
        }
        public static void SeedBarbers(ApplicationDbContext context)
        {
            var barber = new Barber()
            {
                FirstName = "Иван",
                LastName = "Георгиев",
                SpecialityId = context.speciality.First(s => s.Type == "Шеф").Id,
                ExperienceYears = 6,
                ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-173-scaled.jpg"
            };
        }
        public static void SeedServices(ApplicationDbContext context)
        {
            var service = new Service()
            {
                ServiceName = "Фейд прическа",
                Description = "Стилна и модерна подстрижка, която придава изчистен и свеж вид. Фейдът се характеризира с плавно преливане на дължините, започвайки от късо или дори нула в долната част, постепенно преминавайки към по-дълга коса отгоре. Подходяща е за всякакви типове коса и може да бъде персонализирана според предпочитанията на клиента – нисък, среден или висок фейд.",
                Price = 15,
                Duration = new TimeSpan(0, 30, 0)
            };
            context.services.Add(service);
        }
        public static void SeedSpecialities(ApplicationDbContext context)
        {
            var speciality = new Speciality()
            {
                Type = "Стажант"
            };
            context.speciality.Add(speciality);
        }
        public static void SeedMessages()
        {
            var message = new Message()
            {
                Email = "gosho@abv.bg",
                Content = "Не ми хареса прическата, която Иван Георгиев ми направи.",
                Date = new DateTime(2024, 6, 15),
                IsRead = false
            };
        }
        public static void SeedFeedbacks(ApplicationDbContext context)
        {
            var feedback = new Feedback()
            {
                UserId = context.users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                BarberId = context.barbers.First(b => b.FirstName == "Калоян" && b.LastName == "Георгиев").Id,
                Rating = 3,
                Comment = "Добро обслужване, но не бях напълно удовлетворен.",
                FeedBackDate = new DateTime(2024, 6, 10, 15, 54, 23)
            };
        }
        //public static void SeedAppointments(ApplicationDbContext context)
        //{
        //    var appointment = new Appointment()
        //    {

        //    }
        //}
    }
}
