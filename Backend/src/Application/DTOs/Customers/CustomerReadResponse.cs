using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Customers;

public sealed record CustomerReadResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
