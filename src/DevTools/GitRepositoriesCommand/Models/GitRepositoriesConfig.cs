namespace DevTools.GitRepositoriesCommand.Models;

public class GitRepositoriesConfig
{
    public GitRepositoriesMetadata Metadata { get; set; } = new();

    public List<GitRepositoryDefinition> Repositories { get; set; } = [];
}

public class GitRepositoriesMetadata
{
    public string? Description { get; set; }

    public string? Version { get; set; }
}

public class GitRepositoryDefinition
{
    public string GitRepositoryUrl { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Branch { get; set; } = string.Empty;

    public string Folder { get; set; } = string.Empty;
}
