using DevTools.GitRepositoriesCommand.Services.Interfaces;

namespace DevTools.GitRepositoriesCommand.Services;

using System.Diagnostics;
using Models;

public class GitRepositoriesPullService : IGitRepositoriesPullService
{
    private readonly List<string> _mainBranches = ["main", "master"];

    private const string GitRevParseAbbrevRefHead = "rev-parse --abbrev-ref HEAD"; // Get the current branch name
    private const string GitPullCommand = "pull";
    private const string GitCheckoutCommand = "checkout";
    private const string GitPullOriginCommand = "pull origin";
    private const string GitStashCommand = "stash";
    private const string GitStashPopCommand = "stash pop";
    private const string GitRevParseVerifyCommand = "rev-parse --verify"; // Verify that a Git revision exists

    public async Task<PullRepositoriesResult> PullAsync(string startDirectory, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = new PullRepositoriesResult();
        var fullStartPath = Path.GetFullPath(startDirectory);

        if (!Directory.Exists(fullStartPath))
        {
            result.FailedRepositories.Add(new RepositoryPullInfo
            {
                Path = fullStartPath,
                ErrorMessage = "Directory not found"
            });
            return result;
        }

        var gitRepositories = FindGitRepositories(fullStartPath);
        result.TotalRepositoriesFound = gitRepositories.Count;

        foreach (var repoPath in gitRepositories)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessRepositoryAsync(repoPath, result, cancellationToken);
        }

        return result;
    }

    private List<string> FindGitRepositories(string startPath)
    {
        var repositories = new List<string>();

        try
        {
            var dirs = Directory.EnumerateDirectories(startPath, ".git", SearchOption.AllDirectories);

            foreach (var gitDir in dirs)
            {
                var repoPath = Directory.GetParent(gitDir)?.FullName;
                if (!string.IsNullOrEmpty(repoPath))
                {
                    repositories.Add(repoPath);
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Warning: Access denied when searching for repositories: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error searching for repositories: {ex.Message}");
        }

        return repositories;
    }

    private async Task ProcessRepositoryAsync(string repoPath, PullRepositoriesResult result, CancellationToken cancellationToken)
    {
        try
        {
            var currentBranch = await RunGitAsync(GitRevParseAbbrevRefHead, repoPath, cancellationToken);

            if (string.IsNullOrWhiteSpace(currentBranch))
            {
                result.FailedRepositories.Add(new RepositoryPullInfo
                {
                    Path = repoPath,
                    ErrorMessage = "Could not determine current branch - repository may be empty or corrupted"
                });
                return;
            }

            currentBranch = currentBranch.Trim();

            // Check if already on main/master
            if (_mainBranches.Contains(currentBranch, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                Console.WriteLine($"[PROCESSING] {repoPath}");
                Console.WriteLine($"Already on {currentBranch} branch");
                Console.WriteLine("Pulling changes...");


                await RunGitAsync(GitPullCommand, repoPath, cancellationToken);

                result.UpdatedRepositories.Add(new RepositoryPullInfo
                {
                    Path = repoPath,
                    CurrentBranch = currentBranch
                });
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"[PROCESSING] {repoPath}");
            Console.WriteLine($"Current branch: {currentBranch}");

            // Stash changes
            Console.WriteLine("Stashing changes...");
            var stashResult = await RunGitAsync(GitStashCommand, repoPath, cancellationToken);
            var hasStashedChanges = !stashResult.Contains("No local changes to save");

            try
            {
                // Switch to master/main
                var masterBranch = await DetermineMasterBranchAsync(repoPath, cancellationToken);
                if (string.IsNullOrEmpty(masterBranch))
                {
                    throw new InvalidOperationException("Could not determine master/main branch. Neither 'master' nor 'main' exists.");
                }

                Console.WriteLine($"Switching to {masterBranch}...");
                await RunGitAsync($"{GitCheckoutCommand} {masterBranch}", repoPath, cancellationToken);

                // Pull latest changes from remote
                Console.WriteLine($"Pulling latest changes from origin/{masterBranch}...");
                await RunGitAsync($"{GitPullOriginCommand} {masterBranch}", repoPath, cancellationToken);

                // Switch back to feature branch
                Console.WriteLine($"Switching back to {currentBranch}...");
                await RunGitAsync($"{GitCheckoutCommand} {currentBranch}", repoPath, cancellationToken);

                // Unstash changes only if we actually stashed something
                if (hasStashedChanges)
                {
                    Console.WriteLine("Unstashing changes...");
                    await RunGitAsync(GitStashPopCommand, repoPath, cancellationToken);
                }

                Console.WriteLine("Successfully updated");

                result.UpdatedRepositories.Add(new RepositoryPullInfo
                {
                    Path = repoPath,
                    CurrentBranch = currentBranch
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during update: {ex.Message}");
                Console.WriteLine("Attempting to restore original branch...");

                try
                {
                    // Try to restore original state
                    await RunGitAsync($"{GitCheckoutCommand} {currentBranch}", repoPath, cancellationToken);
                    if (hasStashedChanges)
                    {
                        await RunGitAsync(GitStashPopCommand, repoPath, cancellationToken);
                    }
                }
                catch (Exception restoreEx)
                {
                    Console.WriteLine($"Warning: Could not fully restore repository state: {restoreEx.Message}");
                }

                result.FailedRepositories.Add(new RepositoryPullInfo
                {
                    Path = repoPath,
                    CurrentBranch = currentBranch,
                    ErrorMessage = ex.Message
                });
            }
        }
        catch (Exception ex)
        {
            result.FailedRepositories.Add(new RepositoryPullInfo
            {
                Path = repoPath,
                ErrorMessage = ex.Message
            });
        }
    }

    private async Task<string?> DetermineMasterBranchAsync(string repoPath, CancellationToken cancellationToken)
    {
        foreach (var branch in _mainBranches)
        {
            try
            {

                var output = await RunGitAsync($"{GitRevParseVerifyCommand} {branch}", repoPath, cancellationToken);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    return branch;
                }
            }
            catch
            {
                // Branch doesn't exist, try next one
            }
        }

        return null;
    }

    private async Task<string> RunGitAsync(string arguments, string workingDirectory, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo("git", arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        try
        {
            process.Start();
            var standardOutput = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var standardError = await process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                // Handle specific non-critical errors gracefully
                if (arguments.Contains("stash pop") && standardError.Contains("No stash entries found"))
                {
                    // It's okay if there's nothing to pop
                    return standardOutput;
                }

                throw new InvalidOperationException($"Git command failed: {arguments}\nError: {standardError}");
            }

            return standardOutput;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to execute git command '{arguments}': {ex.Message}", ex);
        }
    }
}
