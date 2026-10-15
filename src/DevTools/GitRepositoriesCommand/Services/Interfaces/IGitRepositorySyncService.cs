namespace DevTools.GitRepositoriesCommand.Services.Interfaces;

using Models;

public interface IGitRepositorySyncService
{
    Task SyncAsync(IReadOnlyCollection<GitRepositoryDefinition> repositories, CancellationToken cancellationToken);
}
