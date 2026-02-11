namespace SecureSolution2.Models;

public class UserConfig
{
    public string WatchDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Watch");
    public int TimeoutSeconds { get; set; } = 60;
    public int DefaultRetryCount { get; set; } = 1;
    public string QueueDirectory { get; set; } = string.Empty;    // ← default

    public Dictionary<string, SubSystemMap> Mapping { get; set; }
        = new();
    public class SubSystemMap
    {
        public string ExecutablePath { get; set; } = "";
        public string OutputDirectory { get; set; } = "";
    }
}
