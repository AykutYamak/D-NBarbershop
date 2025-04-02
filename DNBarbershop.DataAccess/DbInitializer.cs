using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.DataAccess
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.services.Any())
            {
                var services = new Service[]
           {
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
           };
                foreach (var service in services)
                {
                    await context.services.AddAsync(service);
                }
                await context.SaveChangesAsync();
            }

            if (!context.speciality.Any())
            {
                var specialities = new Speciality[]
                {
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
                };
                foreach (var speciality in specialities)
                {
                    await context.speciality.AddAsync(speciality);
                }
                await context.SaveChangesAsync();

            }
            if (!context.barbers.Any())
            {
                var barbers = new Barber[]
                {
                    new Barber
                    {
                        FirstName = "Иван",
                        LastName = "Георгиев",
                        SpecialityId = context.speciality.First(s=>s.Type == "Шеф").Id,
                        ExperienceYears = 6,
                        ProfilePictureUrl = "/Assets/Images/barber2.jpg"
                    },
                    new Barber
                    {
                        FirstName = "Петър",
                        LastName = "Христов",
                        SpecialityId = context.speciality.First(s => s.Type == "Стажант").Id,
                        ExperienceYears = 0,
                        ProfilePictureUrl = "/Assets/Images/barber1.jpg",
                    },
                    new Barber
                    {
                        FirstName = "Калоян",
                        LastName = "Георгиев",
                        SpecialityId = context.speciality.First(s => s.Type == "Младши").Id,
                        ExperienceYears = 1,
                        ProfilePictureUrl = "/Assets/Images/barber3.jpg"
                    },
                    new Barber
                    {
                        FirstName = "Христо",
                        LastName = "Петров",
                        SpecialityId = context.speciality.First(s => s.Type == "Шеф").Id,
                        ExperienceYears = 7,
                        ProfilePictureUrl = "/Assets/Images/barber4.jpg"
                    }
                };

                foreach (var barber in barbers)
                {
                    await context.barbers.AddAsync(barber);
                }
                await context.SaveChangesAsync();
            }

            if (!context.feedbacks.Any())
            {
                var feedbacks = new Feedback[]
            {
                    new Feedback
                    {
                        UserId = context.users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                        BarberId = context.barbers.First(b => b.FirstName == "Иван" && b.LastName == "Георгиев").Id,
                        Rating = 5,
                        Comment = "Прекрасно обслужване и страхотен резултат! Препоръчвам на всички!",
                        FeedBackDate = new DateTime(2025, 1, 30, 16, 40, 34)
                    },
                    new Feedback
                    {
                        UserId = context.users.First(u => u.FirstName == "Aykut" && u.LastName == "Yamak").Id,
                        BarberId = context.barbers.First(b => b.FirstName == "Петър" && b.LastName == "Христов").Id,
                        Rating = 4,
                        Comment = "Много добро обслужване, но може би малко повече внимание към детайлите.",
                        FeedBackDate = new DateTime(2025, 2, 15, 10, 34, 12)
                    },
                    new Feedback
                    {
                        UserId = context.users.First(u => u.FirstName == "Иван" && u.LastName == "Иванов").Id,
                        BarberId = context.barbers.First(b => b.FirstName == "Калоян" && b.LastName == "Георгиев").Id,
                        Rating = 3,
                        Comment = "Добро обслужване, но не бях напълно удовлетворен.",
                        FeedBackDate = new DateTime(2024, 6, 10, 15, 54, 23)
                    },
                    new Feedback
                    {
                        UserId = context.users.First(u => u.FirstName == "Aykut" && u.LastName == "Yamak").Id,
                        BarberId = context.barbers.First(b => b.FirstName == "Христо" && b.LastName == "Петров").Id,
                        Rating = 5,
                        Comment = "Невероятно обслужване и страхотен персонал! Препоръчвам на всички!",
                        FeedBackDate = new DateTime(2024, 6, 15, 12, 30, 43)
                    }
            };
                foreach (var feedback in feedbacks)
                {
                    await context.feedbacks.AddAsync(feedback);
                }
                await context.SaveChangesAsync();

            }
            if (!context.messages.Any())
            {
                var messages = new Message[]
                {
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
                };
                foreach (var message in messages)
                {
                    await context.messages.AddAsync(message);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
