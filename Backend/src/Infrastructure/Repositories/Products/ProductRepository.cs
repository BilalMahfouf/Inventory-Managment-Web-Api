using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Products;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
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
    }
}
