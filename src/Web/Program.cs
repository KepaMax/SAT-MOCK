using EXAM_SYSTEM.Infrastructure.Data;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.Host.UseSerilog();

// Configure Forwarded Headers for Caddy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear(); // Clears internal Docker IP restrictions
    options.KnownProxies.Clear();
});

var app = builder.Build();

// MUST be the first middleware to ensure the app knows it's using HTTPS
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");

// REMOVED: app.UseHttpsRedirection() - Caddy handles this!

app.UseStaticFiles();

// Ensure CORS is before MapOpenApi and MapScalar
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/scalar"));

app.MapEndpoints();

app.Run();

public partial class Program { }
