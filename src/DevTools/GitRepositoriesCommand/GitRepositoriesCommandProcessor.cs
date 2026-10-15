using System.Text.Json;
using DevTools.GitRepositoriesCommand.Models;
using DevTools.GitRepositoriesCommand.Services;
using DevTools.GitRepositoriesCommand.Services.Interfaces;

namespace DevTools.GitRepositoriesCommand;

public class GitRepositoriesCommandProcessor
{
    private readonly IGitRepositorySyncService _gitRepositorySyncService;
    private readonly IGitRepositoriesPullService _gitRepositoriesPullService;

    public GitRepositoriesCommandProcessor(
        IGitRepositorySyncService gitRepositorySyncService,
        IGitRepositoriesPullService gitRepositoriesPullService)
    {
        _gitRepositorySyncService = gitRepositorySyncService;
        _gitRepositoriesPullService = gitRepositoriesPullService;
    }

    public async Task<bool> SyncAsync(string configPath, string? baseDirectory, CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(configPath);
        var fullBaseDirectory = Path.GetFullPath(baseDirectory ?? Environment.CurrentDirectory);

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
            var repositories = config.Repositories.Select(repository => new GitRepositoryDefinition
            {
                GitRepositoryUrl = repository.GitRepositoryUrl,
                Name = repository.Name,
                Branch = repository.Branch,
                Folder = Path.GetFullPath(repository.Folder, fullBaseDirectory)
            }).ToArray();

            await _gitRepositorySyncService.SyncAsync(repositories, cancellationToken);
            Console.WriteLine($"Processed {config.Repositories.Count} repositories.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Repository sync failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> PullAsync(string? startDirectory, CancellationToken cancellationToken)
    {
        var fullStartPath = Path.GetFullPath(startDirectory ?? Environment.CurrentDirectory);

        if (!Directory.Exists(fullStartPath))
        {
            Console.WriteLine($"Directory not found: {fullStartPath}");
            return false;
        }

        try
        {
            Console.WriteLine();
            Console.WriteLine("*".PadRight(80, '*'));
            Console.WriteLine($"Searching for Git repositories in: {fullStartPath}");
            Console.WriteLine("*".PadRight(80, '*'));
            Console.WriteLine();

            var result = await _gitRepositoriesPullService.PullAsync(fullStartPath, cancellationToken);

            // Print summary
            Console.WriteLine();
            Console.WriteLine("*".PadRight(80, '*'));
            Console.WriteLine("Summary:");
            Console.WriteLine("*".PadRight(80, '*'));
            Console.WriteLine($"Total repositories found:       {result.TotalRepositoriesFound}");
            Console.WriteLine($"Successfully updated:           {result.UpdatedRepositories.Count}");
            Console.WriteLine($"Failed:                         {result.FailedRepositories.Count}");
            Console.WriteLine();

            if (result.UpdatedRepositories.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully Updated (pulled latest changes):");
                Console.ResetColor();
                foreach (var repo in result.UpdatedRepositories)
                {
                    Console.WriteLine($"  {repo.Path} ({repo.CurrentBranch})");
                }
                Console.WriteLine();
            }

            if (result.FailedRepositories.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed:");
                Console.ResetColor();
                foreach (var repo in result.FailedRepositories)
                {
                    Console.WriteLine($"  {repo.Path}");
                    if (!string.IsNullOrEmpty(repo.ErrorMessage))
                    {
                        Console.WriteLine($"    Error: {repo.ErrorMessage}");
                    }
                }
                Console.WriteLine();
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Pull operation failed: {ex.Message}");
            return false;
        }
    }
}
