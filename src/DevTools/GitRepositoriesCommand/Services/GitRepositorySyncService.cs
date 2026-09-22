namespace DevTools.GitRepositoriesCommand.Services;

using System.Diagnostics;
using DevTools.GitRepositoriesCommand.Models;

public class GitRepositorySyncService : IGitRepositorySyncService
{
    public async Task SyncAsync(IReadOnlyCollection<GitRepositoryDefinition> repositories, CancellationToken cancellationToken)
    {
        foreach (var repository in repositories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidateRepository(repository);

            var targetDirectory = Path.Combine(repository.Folder, repository.Name);
            Directory.CreateDirectory(repository.Folder);

            if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine($"Cloning {repository.Name}...");
                await RunGitAsync(
                    $"clone --branch {Quote(repository.Branch)} --single-branch {Quote(repository.GitRepositoryUrl)} {Quote(targetDirectory)}",
                    Environment.CurrentDirectory,
                    cancellationToken);
                continue;
            }

            if (!Directory.Exists(Path.Combine(targetDirectory, ".git")))
            {
                throw new InvalidOperationException($"Target folder exists but is not a git repository: {targetDirectory}");
            }

            Console.WriteLine($"Pulling {repository.Name}...");
            await RunGitAsync($"checkout {Quote(repository.Branch)}", targetDirectory, cancellationToken);
            await RunGitAsync($"pull --ff-only origin {Quote(repository.Branch)}", targetDirectory, cancellationToken);
        }
    }

    private static void ValidateRepository(GitRepositoryDefinition repository)
    {
        if (string.IsNullOrWhiteSpace(repository.GitRepositoryUrl))
        {
            throw new InvalidOperationException("GitRepositoryUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(repository.Name))
        {
            throw new InvalidOperationException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(repository.Branch))
        {
            throw new InvalidOperationException($"Branch is required for repository '{repository.Name}'.");
        }

        if (string.IsNullOrWhiteSpace(repository.Folder))
        {
            throw new InvalidOperationException($"Folder is required for repository '{repository.Name}'.");
        }
    }

    private static async Task RunGitAsync(string arguments, string workingDirectory, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo("git", arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        var standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;

        if (!string.IsNullOrWhiteSpace(standardOutput))
        {
            Console.WriteLine(standardOutput.TrimEnd());
        }

        if (process.ExitCode == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"git {arguments} failed with exit code {process.ExitCode}.{Environment.NewLine}{standardError.Trim()}");
    }

    private static string Quote(string value)
    {
        return $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
    }
}
