using ManufacturingSystem.Data;
using ManufacturingSystem.Middleware;
using ManufacturingSystem.Repositories;
using ManufacturingSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DB Context - 讀取環境變數
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var host = Environment.GetEnvironmentVariable("DB_URL");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var user = Environment.GetEnvironmentVariable("DB_USER");
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    var connStr = $"Host={host};Port=5432;Database={dbName};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    options.UseNpgsql(connStr);
});

// =========================
// Repositories & Services
// =========================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// =========================
// HttpClient (for Gemini API)
// =========================
builder.Services.AddHttpClient();

// =========================
// CORS 設定
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "https://manufacturing-system-asp-net-latest.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =========================
// Controllers / Swagger
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =========================
// Middleware
// =========================
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

// SPA fallback
app.MapFallbackToFile("index.html");

// Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
