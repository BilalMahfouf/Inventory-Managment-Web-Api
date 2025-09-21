using Application.Abstractions.Repositories.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories.Customers
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        //Task<int> GetAllCustomersAsync(CancellationToken cancellationToken = default);
        //Task<int> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
    }
}
