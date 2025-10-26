using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries;

public interface ITransferQueries
{
    Task<Result<object>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
