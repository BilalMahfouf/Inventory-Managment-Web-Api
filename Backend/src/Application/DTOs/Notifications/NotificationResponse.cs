using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Notifications;

public sealed record NotificationResponse
{
    public Guid Id { get; init; }
    public string EventType { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Severity { get;init; } = null!; 
    public string Message { get; init; } = null!;
    public object Data { get; init; } = null!;
    public DateTime CreatedAt { get; init; }

}
