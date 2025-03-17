using System.Diagnostics.CodeAnalysis;

namespace WebAPI.HealthCheck.Entities;

[ExcludeFromCodeCoverage]
public sealed class HealthInformation
{
    public required string Key { get; set; }
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Status { get; set; }
    public string? Error { get; set; }
}