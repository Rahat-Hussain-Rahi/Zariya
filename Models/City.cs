using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public string Province { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
}
