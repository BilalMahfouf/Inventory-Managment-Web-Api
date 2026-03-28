using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StockMovements.Contracts;

public interface ITransferQueries
{
    Task<Result<object>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
