using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class SearchLog
{
    public int LogId { get; set; }

    public int? PatientId { get; set; }

    public int? SearchCityId { get; set; }

    public string? BloodGroupFilter { get; set; }

    public int? TypeFilter { get; set; }

    public string? AvailabilityFilter { get; set; }

    public short ResultsCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual City? SearchCity { get; set; }

    public virtual DonationType? TypeFilterNavigation { get; set; }
}
