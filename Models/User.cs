using System;
using System.Collections.Generic;

namespace Zariya.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public string? EmailVerifyToken { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpires { get; set; }

    public bool IsActive { get; set; }

    public byte FailedAttempts { get; set; }

    public DateTime? LockedUntil { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AdminLog> AdminLogAdminUsers { get; set; } = new List<AdminLog>();

    public virtual ICollection<AdminLog> AdminLogTargetUsers { get; set; } = new List<AdminLog>();

    public virtual ICollection<Complaint> ComplaintReportedUsers { get; set; } = new List<Complaint>();

    public virtual ICollection<Complaint> ComplaintReporters { get; set; } = new List<Complaint>();

    public virtual ICollection<Complaint> ComplaintResolvedByNavigations { get; set; } = new List<Complaint>();

    public virtual Donor? Donor { get; set; }

    public virtual Patient? Patient { get; set; }
}
