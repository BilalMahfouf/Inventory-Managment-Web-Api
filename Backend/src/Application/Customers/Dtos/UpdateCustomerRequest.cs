using Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Customers.Dtos;

public sealed record UpdateCustomerRequest
{
    public string Name { get; init; } = null!;
    public int? CustomerCategoryId { get; init; }
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public decimal CreditLimit { get; init; }
    public string? PaymentTerms { get; init; }

}
