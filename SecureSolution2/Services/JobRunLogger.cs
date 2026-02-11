using SecureSolution2.Models;
using SecureSolution2.Services;   // for UserConfig

namespace SecureSolution2.Services
{
    internal static class JobRunLogger
    {
        /// <summary>
        /// Creates (or appends) a log file for this batch run.
        /// Now uses the per-day queue directory from UserConfig.
        /// </summary>
        public static StreamWriter Create(UserConfig cfg, Job job)
        {
            // Logs live under the same QueueDirectory where
            // your queue-YYYY-MM-DD.json lives:
            var logDir = Path.Combine(cfg.QueueDirectory, "Logs");
            Directory.CreateDirectory(logDir);

            // One file per RunId
            var fileName = $"{job.RunId}.txt";
            var path = Path.Combine(logDir, fileName);

            return new StreamWriter(path, append: true) { AutoFlush = true };
        }
    }
}
