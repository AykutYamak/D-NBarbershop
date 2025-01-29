using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using DNBarbershop.DataAccess;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Core.Services;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.IService;
using DNBarbershop.Models.Entities;
using DNBarbershop.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Identity.UI.Services;
using DNBarbershop.Utility;
using DNBarbershop.DataAccess.BarberRepository;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        var connection = builder.Configuration.GetConnectionString("HomeConnection");

        //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<ApplicationDbContext>();




        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection,b=>b.MigrationsAssembly("DNBarbershop.DataAccess")));
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped(typeof(IGlobalService<>), typeof(GlobalService<>));
        builder.Services.AddScoped(typeof(IRepository<Speciality>), typeof(Repository<Speciality>));
        builder.Services.AddScoped(typeof(IBarberRepository<Barber>), typeof(BarberRepository<Barber>));
        builder.Services.AddScoped<IAppointmentService, DNBarbershop.Core.Services.AppointmentServices>();
        builder.Services.AddScoped<IBarberService, BarberService>();
        builder.Services.AddScoped<IFeedbackService, FeedbackService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IServiceService, ServiceService>();
        builder.Services.AddScoped<ISpecialityService, SpecialityService>();
        builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<IEmailSender, EmailSender>();

        builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapRazorPages();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();


        app.Run();
        
    }
}