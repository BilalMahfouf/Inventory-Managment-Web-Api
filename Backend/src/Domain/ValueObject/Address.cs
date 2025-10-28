using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObject;

public sealed class Address
{
    public string Street { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string ZipCode { get; private set; } = null!;

    private Address()
    { 
    }
    private Address(string street,
        string city,
        string state,
        string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }
    internal static Address Create(
        string street,
        string city,
        string state,
        string zipCode)
    {
        // Add any necessary validation here
        return new Address(street, city, state, zipCode);
    }

}
