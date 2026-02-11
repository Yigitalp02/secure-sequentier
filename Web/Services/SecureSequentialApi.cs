using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Web.Services
{
    public interface ISecureSequentialApi
    {
        Task<bool> UploadAsync(IFormFile file, string profile, string targetApp, string runId);
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
    }
}
