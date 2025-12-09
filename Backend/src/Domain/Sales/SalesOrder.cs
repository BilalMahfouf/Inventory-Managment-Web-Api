#nullable enable
using Domain;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace Domain.Sales;

public class SalesOrder : AggregateRoot, IBaseEntity
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public DateTime OrderDate { get; private set; }
    public SalesOrderStatus SalesStatus { get; private set; }
    public DateTime? SalesStatusUpdatedAt { get; private set; }

    public decimal TotalAmount { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; private set; } = null!;

    public Customer Customer { get; private set; } = null!;

    private List<SalesOrderItem> _items = new();
    public IReadOnlyCollection<SalesOrderItem> Items => _items.AsReadOnly();
    private SalesOrder()
    {
    }
    private SalesOrder(
        int customerId,
        string? description = null
        )
    {
        CustomerId = customerId;
        Description = description;
    }

    public static SalesOrder Create(
        int customerId,
        List<SalesOrderItem> items,
        SalesOrderStatus salesStatus = SalesOrderStatus.Pending,
        string? description = null
        )
    {
        if(salesStatus is SalesOrderStatus.Cancelled)
        {
            throw new DomainException(
                "New orders cannot be created with Cancelled status.");
        }
        var order = new SalesOrder(
            customerId,
            description);

        order.OrderDate = DateTime.UtcNow;
        order.SalesStatus = salesStatus;
            order.TotalAmount = items.Sum(items => items.LineAmount);
        order._items.AddRange(items);
            order.RaiseDomainEvent(new SalesOrderCreatedDomainEvent(
                order.Id,
                order.CustomerId,
                order.SalesStatus,
                order.OrderDate,
                order.TotalAmount)
                );

        return order;
    }
    public void AddItem(SalesOrderItem item)
    {
        if (SalesStatus is not SalesOrderStatus.Pending)
        {
            throw new DomainException(
                "Items can only be added to orders in Pending status.");
        }
        _items.Add(item);
        TotalAmount += item.LineAmount;
    }
    public void RemoveItem(SalesOrderItem item)
    {
        if (SalesStatus is not SalesOrderStatus.Pending)
        {
            throw new DomainException(
                "Items can only be added to orders in Pending status.");
        }

        if (_items.Any(i => i != item))
        {
            throw new DomainException("Item not found in the order.");
        }
        if (_items.Remove(item))
        {
            TotalAmount -= item.LineAmount;
            return;
        }
        throw new DomainException("Failed to remove item from the order.");
    }

    public void CompleteOrder()
    {
        if (SalesStatus != SalesOrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can be completed.");
        }
        SalesStatus = SalesOrderStatus.Completed;
        SalesStatusUpdatedAt = DateTime.UtcNow;
    }




}