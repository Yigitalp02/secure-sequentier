namespace SecureSolution2.Models;

public class UserConfig
{
    public string WatchDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Watch");
    public int TimeoutSeconds { get; set; } = 60;
    public int DefaultRetryCount { get; set; } = 1;
    public string QueueDirectory { get; set; } = string.Empty;    // ← default

    /// <summary>
    /// Hours to keep user files (inputs, outputs, queue data) before auto-deletion.
    /// Set to 0 to disable cleanup. Default: 1 hour.
    /// </summary>
    public int FileRetentionHours { get; set; } = 1;

    public Dictionary<string, SubSystemMap> Mapping { get; set; }
        = new();
    public class SubSystemMap
    {
        public string ExecutablePath { get; set; } = "";
        public string OutputDirectory { get; set; } = "";
    }
}
