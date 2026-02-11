using SecureSolution2.Hubs;
using SecureSolution2.Models;
using SecureSolution2.Services;
using Serilog;
using Serilog.Sinks.Map;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1) Create configuration service for hot-reload
var configService = new ConfigurationService();
var templateCfg = configService.GetConfig();

// 2) bootstrap Serilog with simplified configuration
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .WriteTo.Console()
  .WriteTo.Map(
    keyPropertyName: "User",
    defaultKey: "none",
    // For each 'User' property value in the log context:
    configure: (user, wtUser) =>
    {
        // Token-replace {USER} in the QueueDirectory to get the root path
        var userDir = templateCfg.QueueDirectory.Replace("{USER}", user);

        wtUser.Map(
          keyPropertyName: "JobId",
          defaultKey: "none",
          // For each 'JobId' property under that User:
          configure: (jobId, wtJob) =>
          {
              // Logs will land in {userDir}\Logs\<jobId>.txt
              var logPath = Path.Combine(userDir, "Logs", $"{jobId}.txt");
              Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

              wtJob.File(
              logPath,
              rollingInterval: RollingInterval.Infinite,
              outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({JobId},{User}) {Message:lj}{NewLine}{Exception}"
            );
          }
        );
    }
  )
  .CreateLogger();

builder.Host.UseSerilog();

// ------------ NEW: register the Serilog.ILogger itself ------------
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// optional: console logging (you can drop this if you want Serilog only)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// per-circuit session
builder.Services.AddScoped<UserSession>();

// SignalR (queue updates)
builder.Services.AddSignalR();

// queue, orchestrator
builder.Services.AddSingleton<IConfigurationService>(configService);
builder.Services.AddSingleton<QueueStore>();
builder.Services.AddHostedService<OrchestratorBackgroundService>();
builder.Services.AddHostedService<CleanupBackgroundService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();          // NEW – explicit routing

// Minimal API: file upload endpoint the Upload page calls
app.MapPost("/api/upload", async (
    HttpContext ctx,
    QueueStore store,
    IWebHostEnvironment env) =>
{
    var req = ctx.Request;
    var form = await req.ReadFormAsync();
    var file = form.Files["file"];
    var targetApp = form["targetApp"].ToString();
    var user = form["user"].ToString();
    var runId = form["runId"].ToString();
    if (string.IsNullOrWhiteSpace(runId))
        runId = Guid.NewGuid().ToString("N");       // auto-generate

    if (file is null || file.Length == 0)
        return Results.BadRequest("No file sent");

    // Put the file into the user's watch directory
    var watchDir = store.GetOrCreateConfig(user).WatchDirectory;
    var inPath = Path.Combine(watchDir, targetApp, user);
    Directory.CreateDirectory(inPath);
    var fullPath = Path.Combine(inPath, file.FileName);

    await using var fs = File.Create(fullPath);
    await file.CopyToAsync(fs);

    // Add to queue
    store.Enqueue(fullPath, targetApp, user, runId);
    return Results.Ok();
});

// Download processed outputs as ZIP
app.MapGet("/api/download", (string runId, string user, QueueStore store) =>
{
    // Find the job
    var jobs = store.GetJobsForUser(user);
    var job = jobs.FirstOrDefault(j => j.RunId == runId);

    if (job is null)
        return Results.NotFound("Job not found");

    if (string.IsNullOrEmpty(job.OutputDirectory) || !Directory.Exists(job.OutputDirectory))
        return Results.NotFound("Output files not found or not yet ready");

    var outputFiles = Directory.GetFiles(job.OutputDirectory, "*", SearchOption.AllDirectories);
    if (outputFiles.Length == 0)
        return Results.NotFound("No output files found");

    // Create ZIP in memory
    var memStream = new MemoryStream();
    using (var zip = new ZipArchive(memStream, ZipArchiveMode.Create, leaveOpen: true))
    {
        foreach (var filePath in outputFiles)
        {
            var entryName = Path.GetRelativePath(job.OutputDirectory, filePath);
            zip.CreateEntryFromFile(filePath, entryName);
        }
    }

    memStream.Position = 0;
    var fileName = $"output_{runId[..Math.Min(8, runId.Length)]}.zip";
    return Results.File(memStream, "application/zip", fileName);
});

app.MapHub<QueueHub>("/hubs/queue");

// Health check endpoint for Docker
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

Console.WriteLine("Starting application on http://localhost:9999");
app.Run();