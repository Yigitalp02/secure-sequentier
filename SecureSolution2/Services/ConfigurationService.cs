using Microsoft.Extensions.Options;
using SecureSolution2.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureSolution2.Services;

public interface IConfigurationService
{
    UserConfig GetConfig();
    void ReloadConfig();
    event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;
}

public class ConfigurationChangedEventArgs : EventArgs
{
    public UserConfig NewConfig { get; set; } = null!;
    public UserConfig OldConfig { get; set; } = null!;
}

public class ConfigurationService : IConfigurationService, IDisposable
{
    private readonly string _configPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private UserConfig _currentConfig;
    private FileSystemWatcher? _fileWatcher;
    private readonly object _lock = new object();

    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    public ConfigurationService()
    {
        _configPath = Path.GetFullPath("DefaultConfig.json");
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        
        _currentConfig = LoadConfig();
        SetupFileWatcher();
    }

    public UserConfig GetConfig()
    {
        lock (_lock)
        {
            return _currentConfig;
        }
    }

    public void ReloadConfig()
    {
        lock (_lock)
        {
            var oldConfig = _currentConfig;
            _currentConfig = LoadConfig();
            
            // Notify subscribers about the configuration change
            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs
            {
                OldConfig = oldConfig,
                NewConfig = _currentConfig
            });
        }
    }

    private UserConfig LoadConfig()
    {
        try
        {
            var configJson = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<UserConfig>(configJson, _jsonOptions)
                        ?? throw new InvalidOperationException("DefaultConfig.json invalid");
            return config;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load configuration from {_configPath}: {ex.Message}", ex);
        }
    }

    private void SetupFileWatcher()
    {
        try
        {
            _fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_configPath)!,
                Filter = Path.GetFileName(_configPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _fileWatcher.Changed += OnConfigFileChanged;
        }
        catch (Exception ex)
        {
            // Log error but don't fail startup
            Console.WriteLine($"Warning: Could not setup file watcher for configuration: {ex.Message}");
        }
    }

    private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            // Wait a bit for the file to be fully written
            Thread.Sleep(100);
            ReloadConfig();
            Console.WriteLine($"Configuration reloaded from {e.FullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reloading configuration: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
    }
}
