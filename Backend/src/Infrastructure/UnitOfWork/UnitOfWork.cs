using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Inventories;
using Application.Abstractions.Repositories.Products;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Users.Response;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Inventories;
using Infrastructure.Repositories.Products;
using Infrastructure.Repositories.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UOW
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryManagmentDBContext _context;
        private readonly ICurrentUserService _currentUserService;

        public IBaseRepository<AlertRule> AlertRules { get; }
        public IBaseRepository<AlertType> AlertTypes { get; }
        public IBaseRepository<AuditLog> AuditLogs { get; }
        public IBaseRepository<Customer> Customers { get; }
        public IBaseRepository<CustomerCategory> CustomerCategories { get; }
        public IBaseRepository<CustomerContact> CustomerContacts { get; }
        public IBaseRepository<Image> Images { get; }
        public IInventoryRepository Inventories { get; }
        public IBaseRepository<Location> Locations { get; }
        public IBaseRepository<LocationType> LocationTypes { get; }
        public IProductRepository Products { get; }
        public IBaseRepository<ProductCategory> ProductCategories { get; }
        public IBaseRepository<ProductImage> ProductImages { get; }
        public IBaseRepository<ProductSupplier> ProductSuppliers { get; }
        public IBaseRepository<PurchaseOrder> PurchaseOrders { get; }
        public IBaseRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
        public IBaseRepository<SalesOrder> SalesOrders { get; }
        public IBaseRepository<SalesOrderItem> SalesOrderItems { get; }
        public IBaseRepository<StockMovement> StockMovements { get; }
        public IBaseRepository<StockMovementType> StockMovementTypes { get; }
        public IBaseRepository<StockTransfer> StockTransfers { get; }
        public IBaseRepository<Supplier> Suppliers { get; }
        public IBaseRepository<SupplierContact> SupplierContacts { get; }
        public IBaseRepository<SupplierType> SupplierTypes { get; }
        public IBaseRepository<UnitOfMeasure> UnitOfMeasures { get; }
        public IBaseRepository<User> Users { get; }
        public IBaseRepository<UserRole> UserRoles { get; }
        public IUserSessionRepository UserSessions { get; }
        public IBaseRepository<ConfirmEmailToken> ConfirmEmailTokens { get; }

        public UnitOfWork(InventoryManagmentDBContext context
            , ICurrentUserService currentUserService)
        {
            _context = context;

            AlertRules = new BaseRepository<AlertRule>(_context);
            AlertTypes = new BaseRepository<AlertType>(_context);
            AuditLogs = new BaseRepository<AuditLog>(_context);
            Customers = new BaseRepository<Customer>(_context);
            CustomerCategories = new BaseRepository<CustomerCategory>(_context);
            CustomerContacts = new BaseRepository<CustomerContact>(_context);
            Images = new BaseRepository<Image>(_context);
            Inventories = new InventoryRepository(_context);
            Locations = new BaseRepository<Location>(_context);
            LocationTypes = new BaseRepository<LocationType>(_context);
            Products = new ProductRepository(_context);
            ProductCategories = new BaseRepository<ProductCategory>(_context);
            ProductImages = new BaseRepository<ProductImage>(_context);
            ProductSuppliers = new BaseRepository<ProductSupplier>(_context);
            PurchaseOrders = new BaseRepository<PurchaseOrder>(_context);
            PurchaseOrderItems = new BaseRepository<PurchaseOrderItem>(_context);
            SalesOrders = new BaseRepository<SalesOrder>(_context);
            SalesOrderItems = new BaseRepository<SalesOrderItem>(_context);
            StockMovements = new BaseRepository<StockMovement>(_context);
            StockMovementTypes = new BaseRepository<StockMovementType>(_context);
            StockTransfers = new BaseRepository<StockTransfer>(_context);
            Suppliers = new BaseRepository<Supplier>(_context);
            SupplierContacts = new BaseRepository<SupplierContact>(_context);
            SupplierTypes = new BaseRepository<SupplierType>(_context);
            UnitOfMeasures = new BaseRepository<UnitOfMeasure>(_context);
            Users = new BaseRepository<User>(_context);
            UserRoles = new BaseRepository<UserRole>(_context);
            UserSessions = new UserSessionRepository(_context);
            ConfirmEmailTokens = new BaseRepository<ConfirmEmailToken>(_context);
            _currentUserService = currentUserService;
        }



        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            _setModifiableEntities();
            _setAuditAbleEntities();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private void _setModifiableEntities()
        {
            var modifiedEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IModifiableEntity && e.State == EntityState.Modified)
            .Select(e => e.Entity as IModifiableEntity);

            

            foreach (var entity in modifiedEntities)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedByUserId = _currentUserService.UserId;
            }
        }
        private void _setAuditAbleEntities()
        {
            var addedEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IBaseEntity && e.State == EntityState.Added)
            .Select(e => e.Entity as IBaseEntity);
            foreach (var entity in addedEntities)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedByUserId= _currentUserService.UserId;
            }
        }
    }
}
