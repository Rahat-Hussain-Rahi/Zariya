using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zariya.Migrations
{
    /// <inheritdoc />
    public partial class SeedDonationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city_name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    province = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cities__031491A88C4C33C5", x => x.city_id);
                });

            migrationBuilder.CreateTable(
                name: "Donation_Types",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donation__2C000598D47A9E30", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    is_email_verified = table.Column<bool>(type: "bit", nullable: false),
                    email_verify_token = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    reset_token = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    reset_token_expires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    failed_attempts = table.Column<byte>(type: "tinyint", nullable: false),
                    locked_until = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_login = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__B9BE370FFC9EE8A4", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Admin_Logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    admin_user_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    target_user_id = table.Column<int>(type: "int", nullable: true),
                    target_table = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    target_record_id = table.Column<int>(type: "int", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ip_address = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin_Lo__9E2397E0E711467E", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_AdminLogs_AdminUser",
                        column: x => x.admin_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_AdminLogs_TargetUser",
                        column: x => x.target_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    complaint_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reporter_id = table.Column<int>(type: "int", nullable: false),
                    reported_user_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "open"),
                    admin_notes = table.Column<string>(type: "text", nullable: true),
                    resolved_by = table.Column<int>(type: "int", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Complain__A771F61C227D8028", x => x.complaint_id);
                    table.ForeignKey(
                        name: "FK_Complaints_ReportedUser",
                        column: x => x.reported_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Complaints_Reporter",
                        column: x => x.reporter_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Complaints_ResolvedBy",
                        column: x => x.resolved_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Donors",
                columns: table => new
                {
                    donor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    cnic = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    gender = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: true),
                    blood_group = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    donation_type_id = table.Column<int>(type: "int", nullable: true),
                    availability = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    is_admin_verified = table.Column<bool>(type: "bit", nullable: false),
                    last_donated = table.Column<DateOnly>(type: "date", nullable: true),
                    total_donations = table.Column<short>(type: "smallint", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    profile_photo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donors__8B5B10F932A3880A", x => x.donor_id);
                    table.ForeignKey(
                        name: "FK_Donors_Cities",
                        column: x => x.city_id,
                        principalTable: "Cities",
                        principalColumn: "city_id");
                    table.ForeignKey(
                        name: "FK_Donors_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    patient_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    cnic = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    gender = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: true),
                    blood_group = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    medical_condition = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    urgency_level = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "normal"),
                    profile_photo = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patients__4D5CE476407BE3D9", x => x.patient_id);
                    table.ForeignKey(
                        name: "FK_Patients_Cities",
                        column: x => x.city_id,
                        principalTable: "Cities",
                        principalColumn: "city_id");
                    table.ForeignKey(
                        name: "FK_Patients_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Donor_Availability_Logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    donor_id = table.Column<int>(type: "int", nullable: false),
                    is_available = table.Column<bool>(type: "bit", nullable: false),
                    changed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donor_Av__9E2397E08E9DCA58", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_AvailabilityLogs_Donors",
                        column: x => x.donor_id,
                        principalTable: "Donors",
                        principalColumn: "donor_id");
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    donor_id = table.Column<int>(type: "int", nullable: true),
                    urgency = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "normal"),
                    blood_group_needed = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "pending"),
                    donor_response = table.Column<string>(type: "text", nullable: true),
                    responded_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_fulfilled = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Requests__18D3B90F99CE93C1", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_Requests_Donors",
                        column: x => x.donor_id,
                        principalTable: "Donors",
                        principalColumn: "donor_id");
                    table.ForeignKey(
                        name: "FK_Requests_Patients",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "patient_id");
                });

            migrationBuilder.CreateTable(
                name: "Search_Logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: true),
                    search_city_id = table.Column<int>(type: "int", nullable: true),
                    blood_group_filter = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    type_filter = table.Column<int>(type: "int", nullable: true),
                    availability_filter = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    results_count = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Search_L__9E2397E0F319E3C3", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_SearchLogs_Cities",
                        column: x => x.search_city_id,
                        principalTable: "Cities",
                        principalColumn: "city_id");
                    table.ForeignKey(
                        name: "FK_SearchLogs_DonationTypes",
                        column: x => x.type_filter,
                        principalTable: "Donation_Types",
                        principalColumn: "type_id");
                    table.ForeignKey(
                        name: "FK_SearchLogs_Patients",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    donation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    donor_id = table.Column<int>(type: "int", nullable: false),
                    patient_id = table.Column<int>(type: "int", nullable: true),
                    request_id = table.Column<int>(type: "int", nullable: true),
                    donation_type_id = table.Column<int>(type: "int", nullable: false),
                    donation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    hospital_name = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    city_id = table.Column<int>(type: "int", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_verified = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donation__296B91DCC32BB79C", x => x.donation_id);
                    table.ForeignKey(
                        name: "FK_Donations_Cities",
                        column: x => x.city_id,
                        principalTable: "Cities",
                        principalColumn: "city_id");
                    table.ForeignKey(
                        name: "FK_Donations_DonationTypes",
                        column: x => x.donation_type_id,
                        principalTable: "Donation_Types",
                        principalColumn: "type_id");
                    table.ForeignKey(
                        name: "FK_Donations_Donors",
                        column: x => x.donor_id,
                        principalTable: "Donors",
                        principalColumn: "donor_id");
                    table.ForeignKey(
                        name: "FK_Donations_Patients",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "patient_id");
                    table.ForeignKey(
                        name: "FK_Donations_Requests",
                        column: x => x.request_id,
                        principalTable: "Requests",
                        principalColumn: "request_id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__7A6B2B8C18412D54", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK_Feedback_Requests",
                        column: x => x.request_id,
                        principalTable: "Requests",
                        principalColumn: "request_id");
                });

            migrationBuilder.InsertData(
                table: "Donation_Types",
                columns: new[] { "type_id", "created_at", "description", "is_active", "type_name" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monetary donation", true, "Money" },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clothing donation", true, "Clothes" },
                    { 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Food donation", true, "Food" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_Logs_admin_user_id",
                table: "Admin_Logs",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_Logs_target_user_id",
                table: "Admin_Logs",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_reported_user_id",
                table: "Complaints",
                column: "reported_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_reporter_id",
                table: "Complaints",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_resolved_by",
                table: "Complaints",
                column: "resolved_by");

            migrationBuilder.CreateIndex(
                name: "UQ__Donation__543C4FD9819D66E6",
                table: "Donation_Types",
                column: "type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_city_id",
                table: "Donations",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_donation_type_id",
                table: "Donations",
                column: "donation_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_donor_id",
                table: "Donations",
                column: "donor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_patient_id",
                table: "Donations",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_request_id",
                table: "Donations",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donor_Availability_Logs_donor_id",
                table: "Donor_Availability_Logs",
                column: "donor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_city_id",
                table: "Donors",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Donors__35BD76A449E15B70",
                table: "Donors",
                column: "cnic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Donors__B9BE370E7689B7EA",
                table: "Donors",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Feedback__18D3B90E2B1133A2",
                table: "Feedback",
                column: "request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_city_id",
                table: "Patients",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Patients__35BD76A4F4665A44",
                table: "Patients",
                column: "cnic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Patients__B9BE370EC53E044E",
                table: "Patients",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_donor_id",
                table: "Requests",
                column: "donor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_patient_id",
                table: "Requests",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Search_Logs_patient_id",
                table: "Search_Logs",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Search_Logs_search_city_id",
                table: "Search_Logs",
                column: "search_city_id");

            migrationBuilder.CreateIndex(
                name: "IX_Search_Logs_type_filter",
                table: "Search_Logs",
                column: "type_filter");

            migrationBuilder.CreateIndex(
                name: "UQ__tmp_ms_x__AB6E616489B3D586",
                table: "Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin_Logs");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "Donor_Availability_Logs");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Search_Logs");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Donation_Types");

            migrationBuilder.DropTable(
                name: "Donors");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
