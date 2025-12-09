using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Customers;

public sealed record CustomerReadResponse : BaseReadResponse
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public int? CustomerCategoryId { get; init; }
    public string? CustomerCategoryName { get; init; } = string.Empty;
    public bool IsActive { get; init; }

    // to do make address in separate object
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;


    public decimal CreditLimit { get; init; }
    public string CreditStatus { get; init; } = string.Empty;
    public string PaymentTerm { get; init; } = string.Empty;

    

}
