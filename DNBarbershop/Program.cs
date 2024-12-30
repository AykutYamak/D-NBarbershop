using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using DNBarbershop.DataAccess;
using DNBarbershop.DataAccess.Repository;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        var connection = builder.Configuration.GetConnectionString("HomeConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
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

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();


        app.Run();
        
    }
}