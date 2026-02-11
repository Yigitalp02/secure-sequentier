using Microsoft.Extensions.Hosting;
using SecureSolution2.Models;
using Serilog;
using SerilogLogger = Serilog.ILogger;

namespace SecureSolution2.Services;

/// <summary>
/// Periodically deletes user files (inputs, outputs, queue data) older than
/// the configured retention period (FileRetentionHours in DefaultConfig.json).
/// </summary>
public sealed class CleanupBackgroundService : BackgroundService
{
    private readonly IConfigurationService _configService;
    private readonly SerilogLogger _log;

    public CleanupBackgroundService(IConfigurationService configService, SerilogLogger log)
    {
        _configService = configService;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run cleanup every 15 minutes
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cfg = _configService.GetConfig();
                if (cfg.FileRetentionHours > 0)
                {
                    var cutoff = DateTime.Now.AddHours(-cfg.FileRetentionHours);
                    _log.Information("Running cleanup: deleting files older than {CutoffTime} ({RetentionHours}h retention)",
                        cutoff, cfg.FileRetentionHours);

                    CleanDirectory(cfg.WatchDirectory, cutoff);

                    // Clean each mapping's output directory
                    foreach (var map in cfg.Mapping.Values)
                    {
                        // OutputDirectory has {USER} placeholder - clean the parent
                        var parentDir = GetParentBeforeUserPlaceholder(map.OutputDirectory);
                        if (!string.IsNullOrEmpty(parentDir))
                            CleanDirectory(parentDir, cutoff);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error during cleanup");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

    /// <summary>
    /// Get the parent directory before the {USER} placeholder.
    /// E.g., "/app/data/processed/{USER}/signatures" → "/app/data/processed"
    /// </summary>
    private static string? GetParentBeforeUserPlaceholder(string path)
    {
        var idx = path.IndexOf("{USER}", StringComparison.OrdinalIgnoreCase);
        if (idx <= 0) return null;

        var parent = path[..idx].TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return string.IsNullOrEmpty(parent) ? null : parent;
    }

    private void CleanDirectory(string directory, DateTime cutoff)
    {
        // Replace any remaining {USER} placeholder for scanning all users
        var baseDir = directory.Contains("{USER}")
            ? GetParentBeforeUserPlaceholder(directory)
            : directory;

        if (string.IsNullOrEmpty(baseDir) || !Directory.Exists(baseDir))
            return;

        try
        {
            // Delete old files
            foreach (var file in Directory.GetFiles(baseDir, "*", SearchOption.AllDirectories))
            {
                try
                {
                    var lastWrite = File.GetLastWriteTime(file);
                    if (lastWrite < cutoff)
                    {
                        File.Delete(file);
                        _log.Debug("Deleted old file: {FilePath}", file);
                    }
                }
                catch (Exception ex)
                {
                    _log.Warning(ex, "Failed to delete file: {FilePath}", file);
                }
            }

            // Delete empty directories (bottom-up)
            CleanEmptyDirectories(baseDir);
        }
        catch (Exception ex)
        {
            _log.Warning(ex, "Failed to clean directory: {Directory}", baseDir);
        }
    }

    private void CleanEmptyDirectories(string directory)
    {
        foreach (var subDir in Directory.GetDirectories(directory))
        {
            CleanEmptyDirectories(subDir);

            try
            {
                if (!Directory.EnumerateFileSystemEntries(subDir).Any())
                {
                    Directory.Delete(subDir);
                    _log.Debug("Removed empty directory: {Directory}", subDir);
                }
            }
            catch { /* ignore - might be in use */ }
        }
    }
}

