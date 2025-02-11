using ChatAppBackend.Data;
using ChatAppBackend.Repositories.Implementations;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Implementations;
using ChatAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ChatAppBackend.Models;
using ChatAppBackend.POCO;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers (ensures API endpoints work)
builder.Services.AddControllers();

// Enable Swagger (API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject Database Context (SQLite example)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IHttpContextAccessor (used in your services)
builder.Services.AddHttpContextAccessor();

// Register password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register Repositories (Data Access Layer)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserChatRepository, UserChatRepository>();

// Register Services (Business Logic Layer)
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserChatService, UserChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register JwtOptions from Configuration**
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Get JwtSettings for Authentication**
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

if (jwtSettings == null)
    throw new ArgumentException("Couldn't load jwt secret key");

// Configure Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

// Register TokenService**
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Enable Swagger UI (for development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware setup:
app.UseHttpsRedirection(); // Enable HTTPS Redirection
app.UseRouting(); // Routing middleware (still required for non-minimal APIs)
app.UseAuthentication(); // Enables JWT authentication
app.UseAuthorization(); // Enable role-based authorization
app.MapControllers(); // Maps API routes to controllers automatically

app.Run();