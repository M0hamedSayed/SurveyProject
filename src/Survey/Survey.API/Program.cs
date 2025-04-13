using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Survey.API;
using Survey.API.Middlewares;
using Survey.Application;
using Survey.Domain.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Infrastructure;
using Survey.Infrastructure.DatabaseContext;
using Survey.Infrastructure.Extensions;
using Survey.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Apply Serilog configuration using the extension method
//builder.Host.UseSerilogLogging();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.AllowEmptyInputInBodyModelBinding = true;
    //Authorization policy
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});


// DependencyInjection
builder.Services
    .AddInfrastructureService(builder.Configuration)
    .AddApplicationServices()
    .AddApiServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["https://localhost:4200", "http://localhost:4200"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Optional if you're using cookies
    });
});


var app = builder.Build();

//sedding data
using (var scope = app.Services.CreateScope())
{
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var domainService = scope.ServiceProvider.GetRequiredService<IUserDomainService>();
    await DataSeed.SeedAsync(domainService, unitOfWork);

    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// error handler middleware
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
