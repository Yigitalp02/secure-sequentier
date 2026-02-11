namespace SecureSolution2.Models;

public class Job
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string TargetApp { get; init; } = default!;
    public string User { get; init; } = default!;

    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> InputPaths { get; init; } = new();

    // NEW: each file with its own status
    public List<JobFile> Files { get; init; } = new();

    // All files in the same batch share this ID (GUID string)
    public string RunId { get; init; } = string.Empty;

    public JobStatus Status { get; set; } = JobStatus.Pending;
    public DateTime? StartedAt { get; set; }
    public int RetryCount { get; set; }
}
