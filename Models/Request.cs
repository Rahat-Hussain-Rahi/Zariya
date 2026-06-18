using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int PatientId { get; set; }

    public int? DonorId { get; set; }

    public string Urgency { get; set; } = null!;

    public string? BloodGroupNeeded { get; set; }

    public string? Message { get; set; }

    public string Status { get; set; } = null!;

    public string? DonorResponse { get; set; }

    public DateTime? RespondedAt { get; set; }

    public bool IsFulfilled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual Donor? Donor { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
