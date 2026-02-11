using Microsoft.Extensions.Options;
using SecureSolution2.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SecureSolution2.Services;

public class QueueStore
{
    // ─────────────── in-memory state ───────────────
    private readonly ConcurrentDictionary<string, List<Job>> _queues = new();

    // ─────────────── serialization setup ───────────
    private readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // ─────────────── injected configuration service ──────────────
    private readonly IConfigurationService _configService;

    public QueueStore(IConfigurationService configService)
    {
        _configService = configService;
    }

    // ─────────────── helpers ───────────────────────
    private string GetUserDir(string user)
    {
        // Reuse the same placeholder logic as in GetOrCreateConfig
        var cfg = GetOrCreateConfig(user);
        var dir = cfg.QueueDirectory;
        Directory.CreateDirectory(dir);
        return dir;
    }

    /// <summary>
    /// Returns something like ".../alice/queue-2025-08-04.json",
    /// creating the folder and seeding "[]" if missing.
    /// </summary>
    private string QueuePath(string user)
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var file = Path.Combine(GetUserDir(user), $"queue-{today}.json");
        if (!File.Exists(file))
            File.WriteAllText(file, "[]");
        return file;
    }

    // ─────────────── public API ────────────────────
    public IEnumerable<(string User, List<Job> Jobs)> EnumerateUserQueues() =>
        _queues.Select(kv => (kv.Key, kv.Value));

    public UserConfig GetOrCreateConfig(string user)
    {
        // Get the current configuration (always fresh from the service)
        var templateCfg = _configService.GetConfig();
        
        // deep‐clone template
        var cfg = JsonSerializer
            .Deserialize<UserConfig>(JsonSerializer.Serialize(templateCfg, _json), _json)!;

        // inject {USER} everywhere
        cfg.WatchDirectory = cfg.WatchDirectory.Replace("{USER}", user);
        cfg.QueueDirectory = cfg.QueueDirectory.Replace("{USER}", user);   // ← new
        foreach (var map in cfg.Mapping.Values)
            map.OutputDirectory = map.OutputDirectory.Replace("{USER}", user);

        return cfg;
    }


    public IEnumerable<Job> GetJobsForUser(string user) =>
        _queues.GetOrAdd(user, _ => LoadQueue(user));

    public void Enqueue(string input, string tgt, string user, string runId)
    {
        var list = _queues.GetOrAdd(user, _ => LoadQueue(user));

        // try to reuse an existing batch that has same runId + targetApp
        var job = list.FirstOrDefault(j => j.RunId == runId && j.TargetApp == tgt);

        if (job is null)
        {
            job = new Job
            {
                RunId = runId,
                TargetApp = tgt,
                User = user,
                InputPaths = new List<string>(),   // legacy (can delete later)
                Files = new List<JobFile>()   // NEW list
            };
            list.Add(job);
        }

        // add path to both collections
        job.InputPaths.Add(input);                     // keep for old JSON readers
        job.Files.Add(new JobFile                // per-file status
        {
            Path = input,
            Status = JobStatus.Pending,
            Retries = 0          // NEW

        });

        SaveQueue(user, list);
    }


    public bool TryDequeuePending(string user, [NotNullWhen(true)] out Job? job)
    {
        var list = _queues.GetOrAdd(user, _ => LoadQueue(user));
        job = list.FirstOrDefault(j => j.Status == JobStatus.Pending);
        if (job is null) return false;

        job.Status = JobStatus.Processing;
        job.StartedAt = DateTime.Now;
        SaveQueue(user, list);
        return true;
    }


    public void Complete(Job job, JobStatus status)
    {
        job.Status = status;
        SaveQueue(job.User, _queues[job.User]);
    }

    /*────────────────────────── NEW helper ──────────────────────────*/
    /// <summary>
    /// Flushes the current in-memory queue for <paramref name="user"/> to disk.
    /// Call this right after you mutate a Job or JobFile and need that change
    /// to be visible to external readers (e.g. the polling Web UI).
    /// </summary>
    public void Flush(string user)
    {
        if (_queues.TryGetValue(user, out var list))
            SaveQueue(user, list);
    }

    /// <summary>
    /// Flushes every user’s queue.  Mostly useful for diagnostics or
    /// if you batch-update multiple users at once.
    /// </summary>
    public void FlushAll()
    {
        foreach (var (u, list) in _queues)
            SaveQueue(u, list);
    }

    // ─────────────── internals ─────────────────────
    private List<Job> LoadQueue(string user)
    {
        var path = QueuePath(user);
        return File.Exists(path)
             ? JsonSerializer.Deserialize<List<Job>>(File.ReadAllText(path), _json)!
             : new List<Job>();
    }

    // Always write the full list back to today's file
    private void SaveQueue(string user, List<Job> list)
    {
        var path = QueuePath(user);
        File.WriteAllText(path, JsonSerializer.Serialize(list, _json) + Environment.NewLine);
    }

}
