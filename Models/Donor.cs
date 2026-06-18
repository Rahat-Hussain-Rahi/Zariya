using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class Donor
{
    public int DonorId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Cnic { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int? CityId { get; set; }

    public string BloodGroup { get; set; } = null!;

    public int? DonationTypeId { get; set; }

    public string? Availability { get; set; }

    public bool IsAdminVerified { get; set; }

    public DateOnly? LastDonated { get; set; }

    public short TotalDonations { get; set; }

    public string? Bio { get; set; }

    public string? ProfilePhoto { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<DonorAvailabilityLog> DonorAvailabilityLogs { get; set; } = new List<DonorAvailabilityLog>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual User User { get; set; } = null!;
}
