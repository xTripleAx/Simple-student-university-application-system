using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TheTask.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDBContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Default")));

//Services for authentication like logged in user role
builder.Services.AddAuthentication("LoginAuthentication")
    .AddCookie("LoginAuthentication", options =>
    {
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redirect for unauthorized access
        options.LoginPath = "/Moodle/Login"; // Redirect for login
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOnly", policy =>
    {
        policy.RequireRole("Employee");
        policy.RequireAuthenticatedUser();
    });
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "showCertificate",
    pattern: "Employee/ShowCertificate/{applicationId}",
    defaults: new { controller = "Employee", action = "ShowCertificate" }); // Add this line for the custom route

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();