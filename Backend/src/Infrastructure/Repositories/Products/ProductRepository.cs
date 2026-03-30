using Application.Shared.Contracts;
using Application.Products.Contracts;
using Domain.Products.Entities;
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
    }
}
