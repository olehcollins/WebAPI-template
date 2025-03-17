using System.Diagnostics.CodeAnalysis;

namespace WebAPI.HealthCheck.Entities;

[ExcludeFromCodeCoverage]
public sealed class HealthResult
{
    public required string Name { get; set; }
    public required string Status { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public required ICollection<HealthInformation> AdditionalInformation { get; set; }
}