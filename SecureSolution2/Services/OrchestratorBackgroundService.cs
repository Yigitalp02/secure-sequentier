using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SecureSolution2.Hubs;
using SecureSolution2.Models;
using Serilog;
using Serilog.Context;
using SerilogLogger = Serilog.ILogger;


namespace SecureSolution2.Services
{
    public sealed class OrchestratorBackgroundService : BackgroundService
    {
        private readonly QueueStore _store;
        private readonly IHubContext<QueueHub> _hub;
        private readonly SerilogLogger _log;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public OrchestratorBackgroundService(
            QueueStore store,
            IHubContext<QueueHub> hub,
            SerilogLogger log)
        {
            _store = store;
            _hub = hub;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var (user, _) in _store.EnumerateUserQueues())
                {
                    if (!_store.TryDequeuePending(user, out var job))
                        continue;

                    await _gate.WaitAsync(stoppingToken);
                    _ = ProcessJobAsync(job, stoppingToken)
                         .ContinueWith(_ => _gate.Release());
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private static string ExpandUser(string path, string user) =>
            path.Replace("{USER}", user, StringComparison.OrdinalIgnoreCase);

        private async Task ProcessJobAsync(Job job, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            // Enrich all logs in this scope
            using (LogContext.PushProperty("JobId", job.Id))
            using (LogContext.PushProperty("User", job.User))
            {
                _log.Information("=== START BATCH for {TargetApp} with {FileCount} files ===",
                                 job.TargetApp, job.Files.Count);
                await QueueHub.Broadcast(_hub, job);

                var cfg = _store.GetOrCreateConfig(job.User);
                var maxRetries = cfg.DefaultRetryCount;

                if (!cfg.Mapping.TryGetValue(job.TargetApp, out var map))
                {
                    _log.Warning("No mapping for {TargetApp}, failing batch", job.TargetApp);
                    CompleteBatch(job, failed: true);
                    return;
                }

                // Build per-batch output folder
                var baseDir = ExpandUser(map.OutputDirectory, job.User);
                var todayDir = Path.Combine(baseDir, DateTime.Now.ToString("dd.MM.yyyy"));
                var batchDir = Path.Combine(todayDir, DateTime.Now.ToString("HH.mm.ss"));
                Directory.CreateDirectory(batchDir);

                // Store output directory in job for download
                job.OutputDirectory = batchDir;
                _store.Flush(job.User);

                // Process each file, retrying up to maxRetries
                foreach (var file in job.Files)
                {
                    bool success = false;
                    for (int attempt = 0; attempt <= maxRetries && !success; attempt++)
                    {
                        // add per-file context
                        using var _ = LogContext.PushProperty("FilePath", file.Path);
                        using var __ = LogContext.PushProperty("Attempt", attempt);

                        file.Status = JobStatus.Processing;
                        file.StartedAt = DateTime.Now;
                        file.Retries = attempt;
                        _store.Flush(job.User);
                        await QueueHub.Broadcast(_hub, job);

                        _log.Information("Processing file (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);

                        // per-file timeout
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        cts.CancelAfter(TimeSpan.FromSeconds(cfg.TimeoutSeconds));

                        try
                        {
                            _log.Information("Executing: {ExecutablePath} with args: \"{InputPath}\" \"{OutputDir}\"", 
                                map.ExecutablePath, file.Path, batchDir);
                            success = await RunWorker(
                                map.ExecutablePath,
                                file.Path,
                                batchDir,
                                cts.Token,
                                _log);
                        }
                        catch (OperationCanceledException)
                        {
                            _log.Warning("Timeout on file {FilePath}", file.Path);
                        }

                        file.Status = success ? JobStatus.Completed : JobStatus.Failed;
                        file.FinishedAt = DateTime.Now;
                        _store.Flush(job.User);
                        await QueueHub.Broadcast(_hub, job);

                        _log.Information("File {FilePath} {Result}", file.Path,
                                         success ? "succeeded" : "failed");
                    }
                }

                // decide overall batch status
                bool allFailed = job.Files.All(f => f.Status == JobStatus.Failed);
                CompleteBatch(job, allFailed);

                sw.Stop();
                _log.Information("=== END BATCH ({ElapsedSeconds}s) ===", sw.Elapsed.TotalSeconds);
            }
        }

        private void CompleteBatch(Job job, bool failed)
        {
            job.Status = failed ? JobStatus.Failed : JobStatus.Completed;
            _store.Flush(job.User);
            _hub.Clients.All.SendAsync("ReceiveJobUpdate", job);
        }

        private static readonly Random _rand = new();
        private static async Task<bool> RunWorker(
            string exe,
            string input,
            string outDir,
            CancellationToken ct,
            SerilogLogger log)
        {
            // simulate warm-up
            await Task.Delay(_rand.Next(5, 16) * 1000, ct);

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"\"{input}\" \"{outDir}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var p = Process.Start(psi)!;

            // asynchronously log stdout
            var stdout = p.StandardOutput;
            var stderr = p.StandardError;
            var outTask = Task.Run(async () =>
            {
                while (!stdout.EndOfStream)
                {
                    var line = await stdout.ReadLineAsync();
                    log.Information("{Line}", line);
                }
            }, ct);

            var errTask = Task.Run(async () =>
            {
                while (!stderr.EndOfStream)
                {
                    var line = await stderr.ReadLineAsync();
                    log.Error("{Line}", line);
                }
            }, ct);

            await p.WaitForExitAsync(ct);
            await Task.WhenAll(outTask, errTask);

            log.Information("Process exited with code {ExitCode}", p.ExitCode);
            return p.ExitCode == 0;
        }
    }
}
