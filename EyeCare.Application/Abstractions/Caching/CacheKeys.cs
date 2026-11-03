namespace EyeCare.Application.Abstractions.Caching;

public static class CacheKeys
{
    public static string Patient(Guid patientId)
        => $"patient:{patientId}";
}
