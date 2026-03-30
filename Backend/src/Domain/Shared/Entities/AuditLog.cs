#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Shared.Entities;

public partial class AuditLog : Entity
{
    public int UserId { get; set; }

    public string? Action { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime? Timestamp { get; set; }
    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}