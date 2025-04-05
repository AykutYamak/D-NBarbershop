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
using Microsoft.AspNetCore.Authentication.Cookies;
using DNBarbershop.DataAccess.AppointmentServiceRepository;
using DNBarbershop.DataAccess.AppointmentRepository;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        var connection = builder.Configuration.GetConnectionString("HomeConnection");




        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection,b=>b.MigrationsAssembly("DNBarbershop.DataAccess")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped(typeof(IGlobalService<>), typeof(GlobalService<>));
        builder.Services.AddScoped(typeof(IRepository<Speciality>), typeof(Repository<Speciality>));
        builder.Services.AddScoped(typeof(IAppointmentServiceRepository<AppointmentServices>), typeof(AppointmentServiceRepository<AppointmentServices>));
        builder.Services.AddScoped(typeof(IAppointmentRepository<Appointment>), typeof(AppointmentRepository<Appointment>));
        builder.Services.AddScoped<IAppointmentService, AppointmentService>();
        builder.Services.AddScoped<IBarberService, BarberService>();
        builder.Services.AddScoped<IFeedbackService, FeedbackService>();
        builder.Services.AddScoped<IServiceService, ServiceService>();
        builder.Services.AddScoped<ISpecialityService, SpecialityService>();
        builder.Services.AddScoped<IAppointmentServiceService, AppointmentServiceService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IMessageService, MessageService>();

        builder.Services.AddIdentity<User, IdentityRole>()
        .AddErrorDescriber<CustomIdentityErrorDescriber>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => { options.LoginPath = "/Account/Login"; options.AccessDeniedPath = "Account/AccessDenied"; });
        

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            await RoleSeeder.Initialize(serviceProvider);
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            DbInitializer.Initialize(context);
        }

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

        app.UseAuthorization(
            //{ options => options.AddPolicy("RequireAdmin", ); }
        );

        app.MapRazorPages();
        
        app.UseStaticFiles();
        
        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();


        app.Run();
        
    }
}