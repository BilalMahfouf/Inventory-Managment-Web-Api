using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Products;
using Domain.Entities.Products;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Products
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(InventoryManagmentDBContext context) : base(context)
        {
        }
        //public async Task<int> GetAllProductsAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _dbSet.CountAsync(p=>!p.IsDeleted,cancellationToken);
        //}
        //public async Task<int> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _dbSet.CountAsync(p => p.IsActive && !p.IsDeleted, cancellationToken);
        //}
    }
}
