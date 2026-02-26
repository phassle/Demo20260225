namespace AcmeApi.Models;

/// <summary>
/// Status DTO - returned from the status endpoint
/// </summary>
public record StatusDto(string Version, string Uptime, string Environment, int ProductCount, int OrderCount);
