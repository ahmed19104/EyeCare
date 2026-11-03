namespace EyeCare.Application.Patients.Queries.SearchPatients;

public sealed record PatientSearchResponse(
    IReadOnlyList<PatientListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record PatientListItem(
    Guid Id,
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string PhoneNumber,
    string? NationalId);