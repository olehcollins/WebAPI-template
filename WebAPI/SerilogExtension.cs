using Serilog;

namespace WebAPI;

public static class SerilogExtension
{
    public static IHostBuilder AddSerilogDocumentation(this IHostBuilder host,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

        return host;
    }

    public static IApplicationBuilder UseSerilogDocumentation(this IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate
                    = "Handled HTTP {RequestMethod} {RequestPath} with Query {RequestQueryString} responded {StatusCode} in {Elapsed:0.0000} ms";
                // If you want to enrich diagnostic context with more request info:
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var queryString = httpContext.Request.QueryString.HasValue
                        ? httpContext.Request.QueryString.Value
                        : "No QueryString Provided";
                    var host = httpContext.Request.Host.HasValue
                        ? httpContext.Request.Host.Value
                        : "No Host";

                    diagnosticContext.Set("RequestHost", host);
                    diagnosticContext.Set("RequestHeaders", httpContext.Request.Headers);
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                    diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
                    diagnosticContext.Set("RequestQueryString", queryString);
                    // You can add more items, but do be mindful of logging sensitive data
                };
            });
        return app;
    }
}