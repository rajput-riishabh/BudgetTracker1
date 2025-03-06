using System.Text;
using BudgetTrackerAPI.Data;
using BudgetTrackerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **Add Database Context (Connection to SQL Server)**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// **Register JwtService**
builder.Services.AddScoped<JwtService>();
// **Configure Authentication**
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Set default authentication scheme
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],      // Issuer from appsettings.json
            ValidAudience = builder.Configuration["JwtSettings:Audience"],    // Audience from appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"])) // Secret key
        };
    });

// **Add Authorization service** (required for [Authorize] attribute to work)
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://localhost:5112") // Add your front-end URLs
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // If you need to send cookies or authorization headers
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); // Removed HTTPS redirection
app.UseCors("AllowSpecificOrigin");
// **Add Authentication Middleware - MUST come before Authorization and MapControllers**
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();