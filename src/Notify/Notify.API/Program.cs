using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Notify.API;
using Notify.API.AppDbContext;
using Notify.API.Events.Realtime;
using Shared.Helpers;
using Shared.Messaging.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Bind configuration settings using IOptions pattern
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("emailSettings"));
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("tokenSettings"));

builder.Services.AddControllers();

// add db 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddServices();

builder.Services
    .addMassTransitConfiguration<ApplicationDbContext>(builder.Configuration, Assembly.GetExecutingAssembly())
    .AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalR(); // Add SignalR
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notifications");

app.MapControllers();

app.Run();