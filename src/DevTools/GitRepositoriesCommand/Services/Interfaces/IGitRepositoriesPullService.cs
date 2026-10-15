using DevTools.GitRepositoriesCommand.Models;

namespace DevTools.GitRepositoriesCommand.Services.Interfaces;

public interface IGitRepositoriesPullService
{
    Task<PullRepositoriesResult> PullAsync(string startDirectory, CancellationToken cancellationToken);
}
