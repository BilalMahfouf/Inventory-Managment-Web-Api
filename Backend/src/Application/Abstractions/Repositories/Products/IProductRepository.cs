using Application.Abstractions.Repositories.Base;
using Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories.Products
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        //Task<int> GetAllProductsAsync(CancellationToken cancellationToken = default);
        //Task<int> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    }
}
