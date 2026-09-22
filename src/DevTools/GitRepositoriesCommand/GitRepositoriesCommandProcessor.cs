using System.Text.Json;
using DevTools.GitRepositoriesCommand.Models;
using DevTools.GitRepositoriesCommand.Services;

namespace DevTools.GitRepositoriesCommand;

public class GitRepositoriesCommandProcessor
{
    private readonly IGitRepositorySyncService _gitRepositorySyncService;

    public GitRepositoriesCommandProcessor(IGitRepositorySyncService gitRepositorySyncService)
    {
        _gitRepositorySyncService = gitRepositorySyncService;
    }

    public async Task<bool> SyncAsync(string configPath, CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(configPath);

        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Config file not found: {fullPath}");
            return false;
        }

        var json = await File.ReadAllTextAsync(fullPath, cancellationToken);
        var config = JsonSerializer.Deserialize<GitRepositoriesConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (config?.Repositories is null || config.Repositories.Count == 0)
        {
            Console.WriteLine("No repositories found in config file.");
            return false;
        }

        try
        {
            await _gitRepositorySyncService.SyncAsync(config.Repositories, cancellationToken);
            Console.WriteLine($"Processed {config.Repositories.Count} repositories.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Repository sync failed: {ex.Message}");
            return false;
        }
    }
}
