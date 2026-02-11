// Services/LocalStorageService.cs – File-based storage service
using System.Text.Json;

namespace SecureSolution2.Services;

public class LocalStorageService
{
    private readonly string _basePath;

    public LocalStorageService()
    {
        _basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureSolution2");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var filePath = Path.Combine(_basePath, $"{key}.json");
        if (!File.Exists(filePath))
            return default;

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var filePath = Path.Combine(_basePath, $"{key}.json");
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public void Remove(string key)
    {
        var filePath = Path.Combine(_basePath, $"{key}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}