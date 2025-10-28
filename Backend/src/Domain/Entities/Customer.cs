#nullable enable
using Domain.Abstractions;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Domain.Entities;

public partial class Customer : IBaseEntity, ISoftDeletable
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;
    public int? CustomerCategoryId { get; private set; }

    public string Email { get; private set; } = null!;

    public string Phone { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public decimal CreditLimit { get; private set; }

    public string? PaymentTerms { get; private set; }

    public CustomerCreditStatus CreditStatus { get; private set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;


    public virtual User? DeletedByUser { get; set; }
    public CustomerCategory? CustomerCategory { get; set; } = null!;

    private Customer()
    {
    }
    private Customer(
        string name,
        int? customerCategoryId,
        string email,
        string phone,
        Address address,
        decimal creditLimit,
        string? paymentTerms
        )
    {
        Name = name;
        CustomerCategoryId = customerCategoryId;
        Email = email;
        Phone = phone;
        Address = address;
        IsActive = true;
        CreditLimit = creditLimit;
        PaymentTerms = paymentTerms;
        CreditStatus = CustomerCreditStatus.Active;
    }
    public static Customer Create(
        string name,
        int? customerCategoryId,
        string email,
        string phone,
        Address address,
        decimal creditLimit,
        string? paymentTerms)
    {
        if (creditLimit < 0)
        {
            throw new DomainException("Credit limit can't be negative ");
        }
        return new Customer(
            name,
            customerCategoryId,
            email,
            phone,
            address,
            creditLimit,
            paymentTerms);
    }
}