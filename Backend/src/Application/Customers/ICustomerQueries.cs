using Application.Customers.Dtos;
using Application.Shared.Paging;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Customers;

public interface ICustomerQueries
{
    Task<Result<object>> GetCustomerSummary(CancellationToken cancellationToken = default);
    Task<Result<CustomerReadResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<Result<PagedList<CustomerTableReadResponse>>> GetAllAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);




}
