#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Users.Entities;

public partial class UserRole : Entity, IModifiableEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public virtual User? DeletedByUser { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public virtual User? UpdatedByUser { get; set; }

}