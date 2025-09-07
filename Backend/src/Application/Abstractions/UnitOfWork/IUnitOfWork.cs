using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Products;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {

        public IBaseRepository<AlertRule> AlertRules { get; }
        public IBaseRepository<AlertType> AlertTypes { get; }
        public IBaseRepository<AuditLog> AuditLogs { get; }
        public IBaseRepository<Customer> Customers { get; }
        public IBaseRepository<CustomerCategory> CustomerCategories { get; }
        public IBaseRepository<CustomerContact> CustomerContacts { get; }
        public IBaseRepository<Inventory> Inventories { get; }
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

        Task <int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
