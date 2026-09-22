namespace DevTools.GitRepositoriesCommand.Services;

using DevTools.GitRepositoriesCommand.Models;

public interface IGitRepositorySyncService
{
    Task SyncAsync(IReadOnlyCollection<GitRepositoryDefinition> repositories, CancellationToken cancellationToken);
}
