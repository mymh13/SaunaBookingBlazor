using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using BookingSystem.Data;
using BookingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookingService>();

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")))
        };
    });
    
// Add Blazored Local Storage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Configuration
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    // Add environment variables in production
    builder.Configuration.AddEnvironmentVariables();
    
    // Override with GitHub Actions secrets if present
    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_KEY")))
    {
        builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_KEY");
    }
    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ADMIN_EMAIL")))
    {
        builder.Configuration["AdminUser:Email"] = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
    }
    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ADMIN_PASSWORD")))
    {
        builder.Configuration["AdminUser:Password"] = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
    }
}

var app = builder.Build();

// Seed the database
await DatabaseSeeder.SeedData(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
