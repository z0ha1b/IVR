using Application.Interfaces;
using Application.Services;
using Infrastructure;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",  // Vite dev server (default)
                  "http://localhost:5174",  // Vite dev server (alternative)
                  "http://localhost:3000",  // Alternative React port
                  "http://localhost:62929", // API HTTP port (for testing)
                  "https://localhost:62928" // API HTTPS port (for testing)
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IVR System API", Version = "v1" });
});

// Register Infrastructure services
builder.Services.AddInfrastructure();

// Register Application services
builder.Services.AddScoped<IIVRMenuService, IVRMenuService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use custom exception handling middleware
app.UseExceptionHandlingMiddleware();

// Enable CORS (must be before UseHttpsRedirection and UseAuthorization)
app.UseCors("AllowReactApp");

// Only redirect to HTTPS in production to avoid CORS issues in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
