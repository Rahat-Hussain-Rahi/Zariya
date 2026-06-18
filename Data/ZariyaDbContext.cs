using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zariya.Models;

namespace Zariya.Data;

public partial class ZariyaDbContext : DbContext
{
    public ZariyaDbContext()
    {
    }

    public ZariyaDbContext(DbContextOptions<ZariyaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<DonationType> DonationTypes { get; set; }

    public virtual DbSet<Donor> Donors { get; set; }

    public virtual DbSet<DonorAvailabilityLog> DonorAvailabilityLogs { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<SearchLog> SearchLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Admin_Lo__9E2397E0E711467E");

            entity.ToTable("Admin_Logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("action");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("ip_address");
            entity.Property(e => e.TargetRecordId).HasColumnName("target_record_id");
            entity.Property(e => e.TargetTable)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("target_table");
            entity.Property(e => e.TargetUserId).HasColumnName("target_user_id");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.AdminLogAdminUsers)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdminLogs_AdminUser");

            entity.HasOne(d => d.TargetUser).WithMany(p => p.AdminLogTargetUsers)
                .HasForeignKey(d => d.TargetUserId)
                .HasConstraintName("FK_AdminLogs_TargetUser");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__031491A88C4C33C5");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("province");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Complain__A771F61C227D8028");

            entity.Property(e => e.ComplaintId).HasColumnName("complaint_id");
            entity.Property(e => e.AdminNotes)
                .HasColumnType("text")
                .HasColumnName("admin_notes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Reason)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.ReportedUserId).HasColumnName("reported_user_id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("open")
                .HasColumnName("status");

            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ComplaintReportedUsers)
                .HasForeignKey(d => d.ReportedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaints_ReportedUser");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ComplaintReporters)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaints_Reporter");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.ComplaintResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("FK_Complaints_ResolvedBy");
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__Donation__296B91DCC32BB79C");

            entity.Property(e => e.DonationId).HasColumnName("donation_id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DonationDate).HasColumnName("donation_date");
            entity.Property(e => e.DonationTypeId).HasColumnName("donation_type_id");
            entity.Property(e => e.DonorId).HasColumnName("donor_id");
            entity.Property(e => e.HospitalName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("hospital_name");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");

            entity.HasOne(d => d.City).WithMany(p => p.Donations)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Donations_Cities");

            entity.HasOne(d => d.DonationType).WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donations_DonationTypes");

            entity.HasOne(d => d.Donor).WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donations_Donors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Donations)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_Donations_Patients");

            entity.HasOne(d => d.Request).WithMany(p => p.Donations)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Donations_Requests");
        });

        modelBuilder.Entity<DonationType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Donation__2C000598D47A9E30");

            entity.ToTable("Donation_Types");

            entity.HasIndex(e => e.TypeName, "UQ__Donation__543C4FD9819D66E6").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("type_name");

            // Seed default donation types used by blood donation workflows.
            entity.HasData(
                new DonationType { TypeId = 1, TypeName = "Whole Blood", Description = "Standard whole blood donation", IsActive = true, CreatedAt = DateTime.Parse("2023-01-01") },
                new DonationType { TypeId = 2, TypeName = "Platelets", Description = "Platelet donation", IsActive = true, CreatedAt = DateTime.Parse("2023-01-01") },
                new DonationType { TypeId = 3, TypeName = "Plasma", Description = "Plasma donation", IsActive = true, CreatedAt = DateTime.Parse("2023-01-01") }
            );
        });

        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.DonorId).HasName("PK__Donors__8B5B10F932A3880A");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_Donors_Availability_Log");
                    tb.HasTrigger("trg_Donors_Update");
                });

            entity.HasIndex(e => e.Cnic, "UQ__Donors__35BD76A449E15B70").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Donors__B9BE370E7689B7EA").IsUnique();

            entity.Property(e => e.DonorId).HasColumnName("donor_id");
            entity.Property(e => e.Availability)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("availability");
            entity.Property(e => e.Bio)
                .HasColumnType("text")
                .HasColumnName("bio");
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("blood_group");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Cnic)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("cnic");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DonationTypeId).HasColumnName("donation_type_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.IsAdminVerified).HasColumnName("is_admin_verified");
            entity.Property(e => e.LastDonated).HasColumnName("last_donated");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePhoto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("profile_photo");
            entity.Property(e => e.TotalDonations).HasColumnName("total_donations");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.City).WithMany(p => p.Donors)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Donors_Cities");

            entity.HasOne(d => d.User).WithOne(p => p.Donor)
                .HasForeignKey<Donor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donors_Users");
        });

        modelBuilder.Entity<DonorAvailabilityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Donor_Av__9E2397E08E9DCA58");

            entity.ToTable("Donor_Availability_Logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("changed_at");
            entity.Property(e => e.DonorId).HasColumnName("donor_id");
            entity.Property(e => e.IsAvailable).HasColumnName("is_available");

            entity.HasOne(d => d.Donor).WithMany(p => p.DonorAvailabilityLogs)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvailabilityLogs_Donors");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__4D5CE476407BE3D9");

            entity.ToTable(tb => tb.HasTrigger("trg_Patients_Update"));

            entity.HasIndex(e => e.Cnic, "UQ__Patients__35BD76A4F4665A44").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Patients__B9BE370EC53E044E").IsUnique();

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("blood_group");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Cnic)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("cnic");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.MedicalCondition)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("medical_condition");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePhoto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("profile_photo");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UrgencyLevel)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("normal")
                .HasColumnName("urgency_level");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.City).WithMany(p => p.Patients)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Patients_Cities");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__18D3B90F99CE93C1");

            entity.ToTable(tb => tb.HasTrigger("trg_Requests_Update"));

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.BloodGroupNeeded)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("blood_group_needed");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DonorId).HasColumnName("donor_id");
            entity.Property(e => e.DonorResponse)
                .HasColumnType("text")
                .HasColumnName("donor_response");
            entity.Property(e => e.IsFulfilled).HasColumnName("is_fulfilled");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.RespondedAt).HasColumnName("responded_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Urgency)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("normal")
                .HasColumnName("urgency");

            entity.HasOne(d => d.Donor).WithMany(p => p.Requests)
                .HasForeignKey(d => d.DonorId)
                .HasConstraintName("FK_Requests_Donors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_Patients");
        });

        modelBuilder.Entity<SearchLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Search_L__9E2397E0F319E3C3");

            entity.ToTable("Search_Logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.AvailabilityFilter)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("availability_filter");
            entity.Property(e => e.BloodGroupFilter)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("blood_group_filter");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ResultsCount).HasColumnName("results_count");
            entity.Property(e => e.SearchCityId).HasColumnName("search_city_id");
            entity.Property(e => e.TypeFilter).HasColumnName("type_filter");

            entity.HasOne(d => d.Patient).WithMany(p => p.SearchLogs)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SearchLogs_Patients");

            entity.HasOne(d => d.SearchCity).WithMany(p => p.SearchLogs)
                .HasForeignKey(d => d.SearchCityId)
                .HasConstraintName("FK_SearchLogs_Cities");

            entity.HasOne(d => d.TypeFilterNavigation).WithMany(p => p.SearchLogs)
                .HasForeignKey(d => d.TypeFilter)
                .HasConstraintName("FK_SearchLogs_DonationTypes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tmp_ms_x__B9BE370FFC9EE8A4");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_Users_AfterInsert_CreateProfile");
                    tb.HasTrigger("trg_Users_Update");
                });

            entity.HasIndex(e => e.Email, "UQ__tmp_ms_x__AB6E616489B3D586").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifyToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email_verify_token");
            entity.Property(e => e.FailedAttempts).HasColumnName("failed_attempts");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.LockedUntil).HasColumnName("locked_until");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reset_token");
            entity.Property(e => e.ResetTokenExpires).HasColumnName("reset_token_expires");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        // Validation for Donation.DonationTypeId before saving
        this.SavingChanges += (sender, args) =>
        {
            var entries = ChangeTracker.Entries<Donation>()
                .Where(e => e.State == EntityState.Added);
            foreach (var entry in entries)
            {
                var typeId = entry.Entity.DonationTypeId;
                var exists = DonationTypes.Any(dt => dt.TypeId == typeId);
                if (!exists)
                {
                    throw new InvalidOperationException($"Invalid DonationTypeId '{typeId}'. Ensure the type exists in Donation_Types.");
                }
            }
        };

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
