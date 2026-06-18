using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int DonorId { get; set; }

    public int? PatientId { get; set; }

    public int? RequestId { get; set; }

    public int DonationTypeId { get; set; }

    public DateOnly DonationDate { get; set; }

    public string? HospitalName { get; set; }

    public int? CityId { get; set; }

    public string? Notes { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual City? City { get; set; }

    public virtual DonationType DonationType { get; set; } = null!;

    public virtual Donor Donor { get; set; } = null!;

    public virtual Patient? Patient { get; set; }

    public virtual Request? Request { get; set; }
}
