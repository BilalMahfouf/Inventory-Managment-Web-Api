using Application.Abstractions.Repositories.Customers;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Customers
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(InventoryManagmentDBContext context) : base(context)
        {
        }

        //public Task<int> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
        //{
        //    return _dbSet.CountAsync(c => c.IsActive && !c.IsDeleted, cancellationToken);
        //}

        //public async Task<int> GetAllCustomersAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _dbSet.CountAsync(c => !c.IsDeleted, cancellationToken);
        //}
    }
}
