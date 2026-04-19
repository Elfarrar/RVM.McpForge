using System.Threading.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using RVM.McpForge.API.Auth;
using RVM.McpForge.API.Health;
using RVM.McpForge.API.Middleware;
using RVM.McpForge.Application.Analyzers;
using RVM.McpForge.Application.Analyzers.Roslyn;
using RVM.McpForge.Application.Generators;
using RVM.McpForge.Application.Planners;
using RVM.McpForge.Application.Services;
using RVM.McpForge.Domain.Interfaces;
using RVM.McpForge.Infrastructure.Data;
using RVM.McpForge.Infrastructure.Repositories;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter());

        var seqUrl = context.Configuration["Seq:ServerUrl"];
        if (!string.IsNullOrEmpty(seqUrl))
            loggerConfiguration.WriteTo.Seq(seqUrl);
    });

    // Controllers + OpenAPI
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Database
    builder.Services.AddDbContext<McpForgeDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Repositories
    builder.Services.AddScoped<IForgeProjectRepository, ForgeProjectRepository>();
    builder.Services.AddScoped<IAnalysisSnapshotRepository, AnalysisSnapshotRepository>();
    builder.Services.AddScoped<IGeneratedMcpProjectRepository, GeneratedMcpProjectRepository>();

    // Application Services
    builder.Services.AddScoped<GitCloneService>();
    builder.Services.AddScoped<RoslynAnalyzer>();
    builder.Services.AddScoped<DatabaseAnalyzer>();
    builder.Services.AddScoped<IToolPlanner, DefaultToolPlanner>();
    builder.Services.AddScoped<IMcpProjectGenerator, ScribanMcpGenerator>();
    builder.Services.AddScoped<ForgeOrchestrator>();

    // Data Protection
    var dataProtectionDir = builder.Configuration["DataProtection:Directory"];
    if (string.IsNullOrEmpty(dataProtectionDir))
        dataProtectionDir = Path.Combine(Path.GetTempPath(), "rvm-mcpforge-dp");
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionDir));

    // Forwarded Headers
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("database");

    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var path = context.Request.Path.Value ?? "";
            if (path.StartsWith("/_blazor") || path.StartsWith("/_framework") ||
                path == "/health" || path.StartsWith("/css") || path.StartsWith("/js"))
                return RateLimitPartition.GetNoLimiter("internal");

            return RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 120,
                    Window = TimeSpan.FromMinutes(1)
                });
        });
    });

    // Authentication
    builder.Services.AddAuthentication(ApiKeyAuthOptions.Scheme)
        .AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>(ApiKeyAuthOptions.Scheme, options =>
        {
            builder.Configuration.GetSection("ApiKeys").Bind(options);
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Auto-create tables
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<McpForgeDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    // PathBase (behind reverse proxy)
    var pathBase = app.Configuration["App:PathBase"];
    if (!string.IsNullOrEmpty(pathBase))
        app.UsePathBase(pathBase);

    // Middleware pipeline
    app.UseForwardedHeaders();
    app.UseStaticFiles();
    app.UseAntiforgery();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    // Routes
    app.MapControllers();
    app.MapRazorComponents<RVM.McpForge.API.Components.App>()
        .AddInteractiveServerRenderMode();
    app.MapHealthChecks("/health").AllowAnonymous();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
