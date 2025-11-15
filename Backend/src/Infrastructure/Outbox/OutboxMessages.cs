using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Outbox;

public class OutboxMessages
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; } = null;
    public string? Errors { get; set; } = null;



}
