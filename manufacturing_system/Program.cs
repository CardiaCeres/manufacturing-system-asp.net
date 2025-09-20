using ManufacturingSystem.Data;
using ManufacturingSystem.Middleware;
using ManufacturingSystem.Repositories;
using ManufacturingSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 讀取環境變數
var dbHost = Environment.GetEnvironmentVariable("DB_URL");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");

// 建立 PostgreSQL 連線字串
var connectionString = $"Host={dbHost};Port=5432;Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true;";

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl ?? "https://manufacturing-system-asp-net-latest.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

// SPA fallback
app.MapFallbackToFile("index.html");

// Swagger (開發環境)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
