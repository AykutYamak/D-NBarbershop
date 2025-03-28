﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.DataAccess;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DNBarbershop.Tests.UnitTests
{
    public class DbSeeder
    {
        public static void SeedDatabase(ApplicationDbContext context)
        {
            SeedSpecialities(context);
            SeedServices(context);
            SeedUsers(context);
            SeedBarbers(context);
            SeedMessages(context);
            SeedFeedbacks(context);
            SeedAppointments(context);
            SeedAppointmentServices(context);

            context.SaveChanges();
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
            context.SaveChanges();
        }
        public static void SeedSpecialities(ApplicationDbContext context)
        {
            var speciality = new Speciality()
            {
                Type = "Стажант"
            };
            context.speciality.Add(speciality);
            context.SaveChanges();
        }
        public static void SeedBarbers(ApplicationDbContext context)
        {
            var barber = new Barber()
            {
                Id = Guid.NewGuid(),
                FirstName = "Иван",
                LastName = "Георгиев",
                SpecialityId = context.speciality.First(s => s.Type == "Стажант").Id,
                ExperienceYears = 6,
                ProfilePictureUrl = "https://thebarbershop.bg/wp-content/uploads/2024/06/the_barber_shop-173-scaled.jpg"
            };
            context.barbers.Add(barber);
            context.SaveChanges();
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
            context.SaveChanges();

        }

        public static void SeedMessages(ApplicationDbContext context)
        {
            var message = new Message()
            {
                Email = "ivan@gmail.com",
                Content = "Не ми хареса прическата, която Иван Георгиев ми направи.",
                Date = new DateTime(2024, 6, 15),
                IsRead = false,
                UserId = context.users.First(u => u.FirstName == "User" && u.LastName == "User").Id
            };
            context.messages.Add(message);
            context.SaveChanges();

        }
        public static void SeedFeedbacks(ApplicationDbContext context)
        {
            var feedback = new Feedback()
            {
                UserId = context.users.First(u => u.FirstName == "User" && u.LastName == "User").Id,
                BarberId = context.barbers.First(b => b.FirstName == "Иван" && b.LastName == "Георгиев").Id,
                Rating = 3,
                Comment = "Добро обслужване, но не бях напълно удовлетворен.",
                FeedBackDate = new DateTime(2024, 6, 10, 15, 54, 23)
            };
            context.feedbacks.Add(feedback);
            context.SaveChanges();

        }
        public static void SeedAppointments(ApplicationDbContext context)
        {
            var appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                UserId = context.users.First(u => u.FirstName == "User" && u.LastName == "User").Id,
                BarberId = context.barbers.First(b => b.FirstName == "Иван" && b.LastName == "Георгиев").Id,
                AppointmentDate = new DateTime(2025, 4, 23),
                AppointmentTime = new TimeSpan(13, 30, 0),
                Status = AppointmentStatus.Scheduled
            };
            context.appointments.Add(appointment);
            context.SaveChanges();

        }
        public static void SeedAppointmentServices(ApplicationDbContext context)
        {
            var appointmentService = new AppointmentServices()
            {
                Id = Guid.NewGuid(),
                AppointmentId = context.appointments.First(u => u.AppointmentDate == new DateTime(2025, 4, 23) && u.AppointmentTime == new TimeSpan(13, 30, 00)).Id,
                ServiceId = context.services.First(u => u.ServiceName == "Фейд прическа").Id
            };
            context.appointmentServices.Add(appointmentService);
            context.SaveChanges();

        }
    }
}
