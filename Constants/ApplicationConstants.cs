namespace Zariya.Constants;

public static class ApplicationConstants
{
    public static class Roles
    {
        public const string Admin = "admin";
        public const string Donor = "donor";
        public const string Patient = "patient";
        public const string Recipient = "recipient";
    }

    public static class SessionKeys
    {
        public const string UserId = "UserId";
        public const string UserEmail = "UserEmail";
        public const string UserRole = "UserRole";
        public const string UserName = "UserName";
    }

    public static class DonationTypes
    {
        public const string WholeBlood = "Whole Blood";
        public const string Platelets = "Platelets";
        public const string Plasma = "Plasma";
    }

    public static class RequestStatus
    {
        public const string Pending = "pending";
        public const string Accepted = "accepted";
        public const string Declined = "declined";
    }

    public static class Availability
    {
        public const string Available = "Available";
        public const string Unavailable = "Unavailable";
    }

    public static readonly IReadOnlyList<string> BloodGroups = new[]
    {
        "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
    };
}
