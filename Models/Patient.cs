using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Cnic { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int? CityId { get; set; }

    public string? BloodGroup { get; set; }

    public string? MedicalCondition { get; set; }

    public string UrgencyLevel { get; set; } = null!;

    public string? ProfilePhoto { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();

    public virtual User User { get; set; } = null!;
}
