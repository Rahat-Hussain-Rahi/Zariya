using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class DonorAvailabilityLog
{
    public int LogId { get; set; }

    public int DonorId { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual Donor Donor { get; set; } = null!;
}
