using Microsoft.EntityFrameworkCore;
using Squido.Models;
using Squido.Services.Implementations;
using Squido.Services.Interfaces;

namespace Squido;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpClient("Squido",
            client => { client.BaseAddress = new Uri(builder.Configuration["Squido:Url"]); });

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped<ICookieService, CookieService>();
        builder.Services.AddDistributedMemoryCache(); //In-memory session store
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddSession(option =>
        {
            option.IdleTimeout = TimeSpan.FromMinutes(60);
            option.Cookie.IsEssential = true;
            option.Cookie.HttpOnly = true;
        });

    builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseRouting();

        

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=HomeMvc}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}