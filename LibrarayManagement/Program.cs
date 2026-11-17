
using LibraryManagement.Data;
using LibraryManagement.Filters;
using LibraryManagement.Interfaces;
using LibraryManagement.Middlewares;
using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Register controllers
builder.Services.AddControllers();

// DATABASE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// IDENTITY
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// JWT AUTHENTICATION
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// AUTHORIZATION
builder.Services.AddAuthorization();


// 2. Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register repository using Dependency Injection
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddScoped<BookValidationFilter>();

var app = builder.Build();

// 5. Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
else
{
    app.UseDeveloperExceptionPage();
}

// 7. HTTPS redirection
app.UseHttpsRedirection();

// 8. Serve static files
app.UseStaticFiles();

// 9. Custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// 10. Inline middleware using Use()
app.Use(async (context, next) =>
{
    Console.WriteLine("Before endpoint execution");

    await next();

    Console.WriteLine("After endpoint execution");
});

// 11. Routing
app.UseRouting();

// 12. CORS
app.UseCors("AngularPolicy");

// 13. Authentication and authorization
 app.UseAuthentication();
//app.UseAuthorization();

// 14. Separate admin pipeline using Map()
app.Map("/admin", adminApp =>
{
    adminApp.Run(async context =>
    {
        await context.Response.WriteAsync("Welcome Admin");
    });
});

// 15. Map controller endpoints
app.MapControllers();

// 16. Start application
app.Run();
