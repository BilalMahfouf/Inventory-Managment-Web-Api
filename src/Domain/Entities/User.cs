#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User : IEntity
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public bool EmailConfirmed { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public virtual User? DeletedByUser { get; set; }
    public virtual UserRole Role { get; set; } = null!;
    public virtual User? UpdatedByUser { get; set; }
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<ConfirmEmailToken> ConfirmEmailTokens { get; set; } = new List<ConfirmEmailToken>();
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Activate()
    {
        IsActive= true;
        UpdatedAt= DateTime.UtcNow;
    }
    public void DesActivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}