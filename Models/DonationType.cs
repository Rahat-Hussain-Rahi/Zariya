using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class DonationType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
}
