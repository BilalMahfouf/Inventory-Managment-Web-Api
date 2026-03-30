#nullable enable
using Domain.Shared.Abstractions;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Domain.Customers.Entities;

public  class Customer : Entity
{
    public string Name { get; private set; } = null!;
    public int? CustomerCategoryId { get; private set; }

    public string Email { get; private set; } = null!;

    public string Phone { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public CustomerCreditStatus CreditStatus { get; private set; }
    public int CreatedByUserId { get; set; }
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
        Address address
        )
    {
        Name = name;
        CustomerCategoryId = customerCategoryId;
        Email = email;
        Phone = phone;
        Address = address;
        IsActive = true;
        CreditStatus = CustomerCreditStatus.Active;
    }
    public static Customer Create(
        string name,
        int? customerCategoryId,
        string email,
        string phone,
        Address address)
    {
        return new Customer(
            name,
            customerCategoryId,
            email,
            phone,
            address);
    }

    public void Update(
        string name,
        int? customerCategoryId,
        string email,
        string phone,
        Address address)
    {
        Name = name;
        CustomerCategoryId = customerCategoryId;
        Phone = phone;
        Address = address;
        UpdateEmail(email);
    }

    private void _EnsureCustomerIsActive()
    {
        if (!this.IsActive)
        {
            throw new DomainException($"Customer must be active to do this action");
        }
        return;
    }
    public void UpdateEmail(string email)
    {
        _EnsureCustomerIsActive();
        if(Email != email)
        {
            // add email validation here if needed
        }
    }
}