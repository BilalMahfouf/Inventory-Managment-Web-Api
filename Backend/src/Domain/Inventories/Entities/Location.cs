#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Inventories.Entities;

public partial class Location : Entity
{
public string Name { get; set; } = null!;

public string Address { get; set; } = null!;

public bool IsActive { get; set; }

public int LocationTypeId { get; set; }
public int CreatedByUserId { get; set; }
public virtual User CreatedByUser { get; set; } = null!;

public virtual User? DeletedByUser { get; set; }

public virtual LocationType LocationType { get; set; } = null!;

}