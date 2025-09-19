using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Manufacturing.Api.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Replace with your real connection string or use configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Server=.;Database=manufacturingDB;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourVerySecretKeyChangeThis!";
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
