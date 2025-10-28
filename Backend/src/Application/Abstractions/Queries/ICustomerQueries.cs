using Application.DTOs.Customers;
using Application.PagedLists;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries;

public interface ICustomerQueries
{
    Task<Result<object>> GetCustomerSummary(CancellationToken cancellationToken = default);
    Task<Result<object>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<Result<PagedList<CustomerTableReadResponse>>> GetAllAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);




}
