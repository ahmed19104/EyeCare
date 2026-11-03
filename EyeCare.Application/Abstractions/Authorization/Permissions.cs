namespace EyeCare.Application.Abstractions.Authorization;

public static class Permissions
{
    public static class Patients
    {
        public const string Read =
            "Patients.Read";

        public const string Create =
            "Patients.Create";

        public const string Update =
            "Patients.Update";
    }

    public static class Appointments
    {
        public const string Read =
            "Appointments.Read";

        public const string Create =
            "Appointments.Create";

        public const string Update =
            "Appointments.Update";

        public const string Cancel =
            "Appointments.Cancel";
    }

    public static class MedicalRecords
    {
        public const string Read =
            "MedicalRecords.Read";

        public const string Create =
            "MedicalRecords.Create";

        public const string Update =
            "MedicalRecords.Update";
    }

    public static class Billing
    {
        public const string Read =
            "Billing.Read";

        public const string Create =
            "Billing.Create";

        public const string Refund =
            "Billing.Refund";
    }

    public static class Branches
    {
        public const string Read =
            "Branches.Read";

        public const string Create =
            "Branches.Create";

        public const string Update =
            "Branches.Update";
    }

    public static class Doctors
    {
        public const string Read =
            "Doctors.Read";

        public const string Create =
            "Doctors.Create";

        public const string Update =
            "Doctors.Update";

        public const string AssignToBranch =
            "Doctors.AssignToBranch";

        public const string ManageSchedule =
            "Doctors.ManageSchedule";
    }

    public static class Users
    {
        public const string Read =
            "Users.Read";

        public const string Create =
            "Users.Create";

        public const string AssignRole =
            "Users.AssignRole";

        public const string AssignBranch =
            "Users.AssignBranch";
    }

    public static readonly IReadOnlyList<string> All =
    [
        Patients.Read,
        Patients.Create,
        Patients.Update,

        Appointments.Read,
        Appointments.Create,
        Appointments.Update,
        Appointments.Cancel,

        MedicalRecords.Read,
        MedicalRecords.Create,
        MedicalRecords.Update,

        Billing.Read,
        Billing.Create,
        Billing.Refund,

        Branches.Read,
        Branches.Create,
        Branches.Update,

        Doctors.Read,
        Doctors.Create,
        Doctors.Update,
        Doctors.AssignToBranch,
        Doctors.ManageSchedule,

        Users.Read,
        Users.Create,
        Users.AssignRole,
        Users.AssignBranch
    ];
}