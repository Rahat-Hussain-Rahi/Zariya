using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public int ReporterId { get; set; }

    public int ReportedUserId { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public string? AdminNotes { get; set; }

    public int? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User ReportedUser { get; set; } = null!;

    public virtual User Reporter { get; set; } = null!;

    public virtual User? ResolvedByNavigation { get; set; }
}
