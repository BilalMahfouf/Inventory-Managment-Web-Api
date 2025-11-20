using Application.Abstractions.Queries;
using Application.Abstractions.Services.Images;
using Application.Abstractions.Services.Product;
using Application.Abstractions.Services.Products;
using Application.Abstractions.Services.User;
using Application.Customers;
using Application.FluentValidations.Products;
using Application.FluentValidations.User;
using Application.FluentValidations.User.Configuration;
using Application.Helpers.Auth;
using Application.Inventories;
using Application.Inventories.DomainEventsHandlers;
using Application.Services.Auth;
using Application.Services.Images;
using Application.Services.Locations;
using Application.Services.Products;
using Application.Services.Shared;
using Application.Services.StockMovements;
using Application.Services.UnitOfMeasures;
using Application.Services.Users;
using Domain.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // fluent validation DI
            services.AddValidatorsFromAssembly(typeof(UserCreateRequestValidator).Assembly);
            services.AddScoped<UserRequestValidatorContainer>();
            services.AddScoped<ProductValidatorContainer>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserRoleService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<UnitOfMeasureService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ImageService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<LocationTypeService>();
            services.AddScoped<LocationService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<StockMovementTypeService>();
            services.AddScoped<StockTransferService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<CustomerCategoryService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(LowStockDomainEventHandler).Assembly);
            });


            return services;
        }
    }
}
