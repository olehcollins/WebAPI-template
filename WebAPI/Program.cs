using Application;
using Infrastructure;
using Presentation;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);


// Conditional configure the Host and Logging services based on the environment
builder.Host.AddSerilogDocumentation(builder.Environment);

// Register your application services
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance
            = $"{context.HttpContext.Request.Method}: {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestID", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddApplication().AddInfrastructure().AddPresentation();
builder.Services.AddSwaggerDocumentation(); // Use the custom Swagger extension method

var app = builder.Build();


// Middleware
app.UseSerilogDocumentation(app.Environment); // Log every request/response first
app.UseHttpsRedirection(); // Enforce HTTPS early in the pipeline
app.UseSwaggerDocumentation(app.Environment); // Register Swagger documentation routes
app.UseExceptionHandler(options => { }); // Exception Handler
app.UseStatusCodePages(); // Use status code pages; update empty API responses

// Minimal API endpoints
app.MapGet("/getUser", () => Results.NotFound());
app.MapGet("/notFound", () => { throw new Exception("An exception occured"); });
app.MapGet("/goodRequest", () => Results.Ok("This is a good request"));

// End of pipeline
app.Run();