namespace DevTools.GitRepositoriesCommand.Models;

public class PullRepositoriesResult
{
    public List<RepositoryPullInfo> UpdatedRepositories { get; set; } = [];

    public List<RepositoryPullInfo> MainBranchRepositories { get; set; } = [];

    public List<RepositoryPullInfo> FailedRepositories { get; set; } = [];

    public int TotalRepositoriesFound { get; set; }

    public bool Success => FailedRepositories.Count == 0;
}

public class RepositoryPullInfo
{
    public string Path { get; set; } = string.Empty;

    public string CurrentBranch { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
