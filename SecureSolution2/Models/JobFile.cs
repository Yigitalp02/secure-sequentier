namespace SecureSolution2.Models;

public class JobFile
{
    public string Path { get; set; } = "";
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    /* ★ add – incremented by the orchestrator */
    public int Retries { get; set; } = 0;
}
