using Application.Shared.Contracts;
using Domain.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Contracts
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        //Task<int> GetAllProductsAsync(CancellationToken cancellationToken = default);
        //Task<int> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    }
}
