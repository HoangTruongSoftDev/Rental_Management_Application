using Microsoft.EntityFrameworkCore;
using RentalManagementFinalProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = new ConfigurationBuilder() // Necessary Code for Connecting to Database based on connection string name in appsettings.json
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
var connectionString = configuration.GetConnectionString("RentalManagementDatabase"); // Necessary Code for Connecting to Database based on connection string name in appsettings.json

builder.Services.AddDbContext<RentalManagementDbContext>( // Necessary Code for Connecting to Database based on connection string name in appsettings.json
    options =>
    {
        options.UseSqlServer(connectionString);
    }
);


builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Necessary Code for Session

builder.Services.AddSession( // Necessary Code for Session
    options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    }
);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Necessary Code for Authentication
    .AddCookie(option =>
    {
        option.LoginPath = "/Access/Login"; // Login Path
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20); //Time expired
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Necessary Code for Authentication

app.UseAuthorization(); // Necessary Code for Authorization
app.UseSession(); // Necessary Code for Session
app.MapControllerRoute(
    name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
