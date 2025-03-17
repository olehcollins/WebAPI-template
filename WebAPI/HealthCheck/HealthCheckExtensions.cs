using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using WebAPI.HealthCheck.Entities;

namespace WebAPI.HealthCheck;

[ExcludeFromCodeCoverage]
public static class HealthCheckExtensions
{
    public static IEndpointConventionBuilder MapCustomHealthChecks(
        this IEndpointRouteBuilder endpoints, string serviceName)
    {
        return endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new HealthResult
                        {
                            Name = serviceName,
                            Status = report.Status.ToString(),
                            TotalDuration = report.TotalDuration,
                            AdditionalInformation = report.Entries.Select(e =>
                                new HealthInformation
                                {
                                    Key = e.Key,
                                    Description = e.Value.Description,
                                    Duration = e.Value.Duration,
                                    Status = Enum.GetName(typeof(HealthStatus),
                                        e.Value.Status),
                                    Error = e.Value.Exception?.Message
                                }).ToList()
                        }, Formatting.None,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            }
        );
    }
}