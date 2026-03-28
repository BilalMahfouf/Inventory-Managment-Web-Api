using Application.Authentication.Contracts;
using Application.Authentication.Services;
using Application.Customers;
using Application.Dashboard.Services;
using Application.Images.Contracts;
using Application.Images.Services;
using Application.Inventories;
using Application.Inventories.DomainEventsHandlers;
using Application.Locations.Services;
using Application.Products.Contracts;
using Application.Products.Services;
using Application.Products.Validators;
using Application.Sales.Services;
using Application.Shared.Services;
using Application.StockMovements.Services;
using Application.UnitOfMeasures.Services;
using Application.Users.Contracts;
using Application.Users.Services;
using Application.Users.Validators;
using Application.Users.Validators.Configuration;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<SalesOrderService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(LowStockDomainEventHandler).Assembly);
            });


            return services;
        }
    }
}
