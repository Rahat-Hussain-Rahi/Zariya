using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class AdminLog
{
    public int LogId { get; set; }

    public int AdminUserId { get; set; }

    public string Action { get; set; } = null!;

    public int? TargetUserId { get; set; }

    public string? TargetTable { get; set; }

    public int? TargetRecordId { get; set; }

    public string? Details { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User AdminUser { get; set; } = null!;

    public virtual User? TargetUser { get; set; }
}
