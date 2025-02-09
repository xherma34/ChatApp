using ChatAppBackend.Data;
using ChatAppBackend.Repositories.Implementations;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Implementations;
using ChatAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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

var app = builder.Build();

// Enable Swagger UI (for development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS Redirection
app.UseHttpsRedirection();

app.UseRouting(); // Routing middleware (still required for non-minimal APIs)
// app.UseAuthorization(); // Enable role-based authorization

// 🔹 Replace UseEndpoints with this
app.MapControllers(); // Maps API routes to controllers automatically

app.Run();