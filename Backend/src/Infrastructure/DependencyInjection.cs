﻿using Application.Abstractions.Auth;
using Application.Abstractions.Queries;
using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Inventories;
using Application.Abstractions.Repositories.Products;
using Application.Abstractions.Services.Email;
using Application.Abstractions.Services.Storage;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.Common.Abstractions;
using Application.Services.Shared;
using Infrastructure.Authentication;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Inventories;
using Infrastructure.Repositories.Products;
using Infrastructure.Repositories.User;
using Infrastructure.Services;
using Infrastructure.Services.Email;
using Infrastructure.Services.ImageStorage;
using Infrastructure.UOW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services
            ,IConfiguration configuration)
        {
            services.Configure<JwtOptions>(options =>
            {
                options.SingingKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set");
                options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER environment variable is not set");
                options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable is not set");
                options.LifeTime = byte.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_LIFETIME_MINUTES") ?? "15");
            });

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            services.AddDbContext<InventoryManagmentDBContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
               
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddSingleton<IImageStorageService, FileSystemImageStorageService>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IDashboardQueries, DashboardQueries>();
            services.AddScoped<IProductQueries, ProductQueries>();

            // Email Options config 
            services.Configure<EmailOptions>(options =>
            {
                options.Port = configuration.GetValue<int>("EMAIL_CONFIGURATIONS:PORT");
                options.Host = configuration.GetValue<string>("EMAIL_CONFIGURATIONS:HOST") ?? throw new InvalidOperationException("EMAIL_CONFIGURATIONS_HOST is not set");
                options.Password= Environment.GetEnvironmentVariable("EMAIL_CONFIGURATIONS_PASSWORD") ?? throw new InvalidOperationException("EMAIL_CONFIGURATIONS_PASSWORD environment variable is not set");
                options.Email = Environment.GetEnvironmentVariable("EMAIL_CONFIGURATIONS_EMAIL") ?? throw new InvalidOperationException("EMAIL_CONFIGURATIONS_EMAIL environment variable is not set");
            });
            return services;

        }
    }
}
