using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Web.Services
{
    public interface ISecureSequentialApi
    {
        Task<bool> UploadAsync(IFormFile file, string profile, string targetApp, string runId);
        Task<(Stream? stream, string? fileName)> DownloadAsync(string runId, string user);
    }

    public sealed class SecureSequentialApi : ISecureSequentialApi
    {
        private readonly HttpClient _http;
        public SecureSequentialApi(HttpClient http) => _http = http;

        public async Task<bool> UploadAsync(IFormFile file, string profile, string targetApp, string runId)
        {
            using var mp = new MultipartFormDataContent();

            // add the file
            await using var stream = file.OpenReadStream();
            var filePart = new StreamContent(stream);
            filePart.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = $"\"{file.FileName}\""
                };
            mp.Add(filePart, "file", file.FileName);

            // add extra fields exactly like the PS-script
            mp.Add(new StringContent(targetApp), "targetApp");
            mp.Add(new StringContent(profile), "user");
            mp.Add(new StringContent(runId), "runId");

            var resp = await _http.PostAsync("api/upload", mp);
            return resp.IsSuccessStatusCode;
        }

        public async Task<(Stream? stream, string? fileName)> DownloadAsync(string runId, string user)
        {
            var resp = await _http.GetAsync($"api/download?runId={Uri.EscapeDataString(runId)}&user={Uri.EscapeDataString(user)}");
            if (!resp.IsSuccessStatusCode)
                return (null, null);

            var stream = await resp.Content.ReadAsStreamAsync();
            var fileName = resp.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                           ?? $"output_{runId[..Math.Min(8, runId.Length)]}.zip";
            return (stream, fileName);
        }
    }
}
