using System;
using System.Collections.Generic;
using System.Linq;    // <— for Sum()

namespace Web.Models
{
    public class FileRecord
    {
        public string Path { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int Retries { get; set; }
    }

    public class RunRecord
    {
        public Guid Id { get; set; }
        public string TargetApp { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public List<FileRecord> Files { get; set; } = new();
        public string RunId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public int RetryCount => Files?.Sum(f => f.Retries) ?? 0;
    }
}
