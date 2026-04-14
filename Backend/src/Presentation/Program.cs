


using DotNetEnv;
using Application;
using Infrastructure;
using Carter;
using Scalar.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddCarter(configurator: c =>
{
    // Validators in this project depend on DbContext (scoped),
    // so Carter must not register them as singleton.
    c.WithDefaultValidatorLifetime(ServiceLifetime.Scoped);
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices();

var allowedOrigins = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.InitData();
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();


app.MapSignalRHubs();

app.Run();
