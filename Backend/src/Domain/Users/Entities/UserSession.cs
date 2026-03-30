#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Users.Entities;

public partial class UserSession : Entity
{
    public int UserId { get; set; }

    public string Token { get; set; } = null!;
    public byte TokenType { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public virtual User User { get; set; } = null!;
}