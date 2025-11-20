using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Customers;

public sealed record CustomerTableReadResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public int? CustomerCategoryId { get; init; }
    public string CustomerCategoryName { get; init; } = null!;
    public int TotalOrders { get; init; }
    public decimal TotalSpent { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }

}
