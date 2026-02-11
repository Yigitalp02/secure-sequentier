using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Web.Models;
using Web.Options;
using Web.Services;
using SecureSolution2.Models;


namespace Web.Controllers
{
    public class FileQueueController : Controller
    {
        private readonly ISecureSequentialApi _api;
        private readonly IWebHostEnvironment _env;
        private readonly WebPathsOptions _opt;
        private readonly UserConfig _templateCfg;
        private readonly JsonSerializerOptions _json = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private const string SessionProfileKey = "ActiveProfile";
        private const string PersistentCookieName = "SecureSequentierUserId";

        public FileQueueController(
            ISecureSequentialApi api,
            IOptions<WebPathsOptions> opt,
            IWebHostEnvironment env)
        {
            _api = api;
            _opt = opt.Value;
            _env = env;

            // 1) Locate and load DefaultConfig.json
            var configPath = Path.IsPathRooted(_opt.DefaultConfig)
                         ? _opt.DefaultConfig
                         : Path.Combine(_env.ContentRootPath, _opt.DefaultConfig);

            var text = System.IO.File.ReadAllText(configPath);
            _templateCfg = JsonSerializer.Deserialize<UserConfig>(text, _json)
                           ?? throw new InvalidOperationException("DefaultConfig.json invalid");
        }

        // ───────── session-backed current user folder ─────────
        private string? ActiveProfile
        {
            get => HttpContext.Session.GetString(SessionProfileKey);
            set
            {
                if (value is null)
                    HttpContext.Session.Remove(SessionProfileKey);
                else
                    HttpContext.Session.SetString(SessionProfileKey, value);
            }
        }

        // ─────────── build a per-user config from the template ───────────
        private UserConfig GetPerUserConfig(string user)
        {
            // deep-clone via serialization
            var cfg = JsonSerializer.Deserialize<UserConfig>(
                          JsonSerializer.Serialize(_templateCfg, _json),
                          _json
                      )!;

            // inject the {USER} token everywhere
            cfg.WatchDirectory = cfg.WatchDirectory.Replace("{USER}", user);
            cfg.QueueDirectory = cfg.QueueDirectory.Replace("{USER}", user);
            foreach (var m in cfg.Mapping.Values)
                m.OutputDirectory = m.OutputDirectory.Replace("{USER}", user);

            return cfg;
        }

        // ─────────── where today’s queue lives ───────────
        private string GetQueuePathForDate(string user, string date)
        {
            var cfg = GetPerUserConfig(user);
            Directory.CreateDirectory(cfg.QueueDirectory);

            var file = Path.Combine(cfg.QueueDirectory, $"queue-{date}.json");
            if (!System.IO.File.Exists(file))
                System.IO.File.WriteAllText(file, "[]");
            return file;
        }

        // ─────────── bootstrap a new user session ───────────
        private string EnsureCurrentProfile()
        {
            string userId;

            // 1) Check for persistent cookie (works across sessions/restarts)
            if (HttpContext.Request.Cookies.TryGetValue(PersistentCookieName, out var cookieId)
                && !string.IsNullOrWhiteSpace(cookieId))
            {
                userId = cookieId;
            }
            else
            {
                // 2) On Windows (local dev), try to use the Windows display name
                string? windowsName = null;
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        windowsName = System.DirectoryServices.AccountManagement
                            .UserPrincipal.Current?.DisplayName;
                    }
                    catch { }

                    // Fallback to system username
                    windowsName ??= HttpContext.User.Identity?.Name?.Split('\\').Last()
                                    ?? Environment.UserName;
                }

                if (!string.IsNullOrWhiteSpace(windowsName))
                {
                    // Use sanitized Windows name
                    userId = windowsName
                        .Replace(':', '_').Replace('\\', '_').Replace('/', '_').Trim();
                }
                else
                {
                    // 3) Generate a random anonymous ID for non-Windows (Docker/Linux)
                    userId = "user-" + Guid.NewGuid().ToString("N")[..8];
                }

                // Store in persistent cookie (365 days)
                HttpContext.Response.Cookies.Append(PersistentCookieName, userId, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(365),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true
                });
            }

            if (ActiveProfile != userId)
                ActiveProfile = userId;

            // Proactively create today's queue file
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            GetQueuePathForDate(userId, today);

            ViewBag.DisplayName = userId;
            return userId;
        }

        private string DefaultConfigPath =>
    Path.IsPathRooted(_opt.DefaultConfig)
        ? _opt.DefaultConfig
        : Path.Combine(_env.ContentRootPath, _opt.DefaultConfig);

        /// <summary>
        /// Read the "Mapping" entries from DefaultConfig.json so the UI can list your sub-apps.
        /// </summary>
        private void LoadSubApps()
        {
            var list = new List<string>();
            try
            {
                using var doc = JsonDocument.Parse(
                    System.IO.File.ReadAllText(DefaultConfigPath)
                );
                if (doc.RootElement.TryGetProperty("Mapping", out var map))
                    foreach (var entry in map.EnumerateObject())
                        list.Add(entry.Name);
            }
            catch { /* ignore */ }

            if (list.Count == 0)
                list.AddRange(new[] { "signer", "successapp" });

            ViewBag.SubApps = list.ToArray();
        }

        /* ─────────────────────────── ROUTES ─────────────────────────── */

        public IActionResult Index()
        {
            EnsureCurrentProfile();
            LoadSubApps();      // ← call it here
            return View();
        }

        [HttpGet]
        public IActionResult Queue(string runId)
        {
            EnsureCurrentProfile();
            LoadSubApps();      // ← and here
            ViewBag.RunId = runId;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string targetApp, List<IFormFile> files)
        {
            var user = EnsureCurrentProfile();
            if (files == null || files.Count == 0)
            {
                ViewBag.Message = "Choose at least one file.";
                LoadSubApps();  // ← and here if you fall back to the Index view
                return View("Index");
            }

            var runId = Guid.NewGuid().ToString("N");
            var success = 0;
            foreach (var f in files)
                if (await _api.UploadAsync(f, user, targetApp, runId))
                    success++;

            TempData["Message"] = $"{success}/{files.Count} file(s) queued successfully.";
            return RedirectToAction(nameof(Queue), new { runId });
        }

        [HttpGet]
        public async Task<IActionResult> Download(string runId)
        {
            var user = EnsureCurrentProfile();
            var (stream, fileName) = await _api.DownloadAsync(runId, user);

            if (stream is null || fileName is null)
            {
                TempData["Message"] = "Output files are not ready yet or have been cleaned up.";
                return RedirectToAction(nameof(Queue), new { runId });
            }

            return File(stream, "application/zip", fileName);
        }

        [HttpGet]
        public IActionResult BatchStatus(string runId)
        {
            var user = EnsureCurrentProfile();
            if (string.IsNullOrWhiteSpace(runId))
                return Json(new { status = "Unknown" });

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var path = GetQueuePathForDate(user, today);
            if (!System.IO.File.Exists(path))
                return Json(new { status = "Unknown" });

            try
            {
                using var doc = JsonDocument.Parse(System.IO.File.ReadAllText(path));
                foreach (var jobEl in doc.RootElement.EnumerateArray())
                {
                    if (jobEl.GetProperty("RunId").GetString() != runId) continue;

                    var status = jobEl.GetProperty("Status").GetString();
                    var startedAt = jobEl.TryGetProperty("StartedAt", out var st)
                                    ? st.GetString()
                                    : null;

                    var files = new List<object>();
                    if (jobEl.TryGetProperty("Files", out var filesEl))
                    {
                        foreach (var f in filesEl.EnumerateArray())
                        {
                            files.Add(new
                            {
                                path = f.GetProperty("Path").GetString(),
                                status = f.GetProperty("Status").GetString(),
                                startedAt = f.TryGetProperty("StartedAt", out var s0) ? s0.GetString() : null,
                                finishedAt = f.TryGetProperty("FinishedAt", out var s1) ? s1.GetString() : null,
                                retries = f.TryGetProperty("Retries", out var r0) ? r0.GetInt32() : 0
                            });
                        }
                    }

                    return Json(new { status, files, startedAt });
                }
            }
            catch { /* ignore */ }

            return Json(new { status = "Unknown" });
        }

        [HttpGet]
        public IActionResult History(
            int page = 1,
            int pageSize = 10,
            string sortField = "StartedAt",
            string sortDir = "desc",
            string? date = null)
        {
            var user = EnsureCurrentProfile();
            var cfg = GetPerUserConfig(user);

            // 1) Gather all dates
            Directory.CreateDirectory(cfg.QueueDirectory);
            var dates = Directory
                .GetFiles(cfg.QueueDirectory, "queue-*.json")
        .Select(fn => Path.GetFileNameWithoutExtension(fn)!)
                .Select(fn => fn.Substring("queue-".Length))// "2025-08-04"
                .OrderByDescending(d => d)
                .ToList();

            // 2) Pick the date
            var selectedDate = date is not null && dates.Contains(date)
                               ? date
                               : DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.AvailableDates = dates;
            ViewBag.SelectedDate = selectedDate;

            // 3) Load that file
            var queuePath = Path.Combine(cfg.QueueDirectory, $"queue-{selectedDate}.json");
            string raw = System.IO.File.Exists(queuePath)
                         ? System.IO.File.ReadAllText(queuePath)
                         : "[]";

            var runs = JsonSerializer.Deserialize<List<RunRecord>>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<RunRecord>();

            // 4) Sort
            runs = (sortField, sortDir.ToLower()) switch
            {
                ("RunId", "asc") => runs.OrderBy(r => r.RunId).ToList(),
                ("RunId", "desc") => runs.OrderByDescending(r => r.RunId).ToList(),
                ("Status", "asc") => runs.OrderBy(r => r.Status).ToList(),
                ("Status", "desc") => runs.OrderByDescending(r => r.Status).ToList(),
                ("Retries", "asc") => runs.OrderBy(r => r.RetryCount).ToList(),
                ("Retries", "desc") => runs.OrderByDescending(r => r.RetryCount).ToList(),
                ("StartedAt", "asc") => runs.OrderBy(r => r.StartedAt).ToList(),
                ("StartedAt", "desc") => runs.OrderByDescending(r => r.StartedAt).ToList(),
                _ => runs.OrderByDescending(r => r.StartedAt).ToList()
            };

            // 5) Page
            var totalPages = (int)Math.Ceiling(runs.Count / (double)pageSize);
            var paged = runs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.DisplayName = ActiveProfile ?? user;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSizes = new[] { 10, 25, 50, 100 };
            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir.ToLower();

            return View(paged);
        }
    }
}
