using Application;
using Infrastructure;
using Presentation;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);


// Conditional configure the Host and Logging services based on the environment
builder.Host.AddSerilogDocumentation(builder.Environment);

// Register your application services
builder.Services.AddOpenApi();
builder.Services.AddApplication().AddInfrastructure().AddPresentation();
builder.Services.AddSwaggerDocumentation(); // Use the custom Swagger extension method

var app = builder.Build();


// 1) Log every request/response first
app.UseSerilogDocumentation(app.Environment);
// 2) Enforce HTTPS early in the pipeline
app.UseHttpsRedirection();
// 3) Register Swagger documentation routes
app.UseSwaggerDocumentation(app.Environment);


// 4) Minimal API endpoints
app.MapGet("/badRequest", () => Results.BadRequest(new { error = "This is a bad request error" }));
app.MapGet("/goodRequest", () => Results.Ok("This is a good request"));

//5) Signal to end the pipeline
app.Run();