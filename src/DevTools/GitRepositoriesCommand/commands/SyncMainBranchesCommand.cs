namespace DevTools.GitRepositoriesCommand.commands;

using System.CommandLine;

public class SyncMainBranchesCommand : Command
{
    public SyncMainBranchesCommand(GitRepositoriesCommandProcessor processor)
        : base("sync-main-branches", "Recursively finds Git repositories, stashes changes on feature branches, pulls master/main, and restores feature branches")
    {
        var directoryArgument = new Argument<string?>("directory")
        {
            Description = "Starting directory to search for repositories (defaults to current directory)",
            Arity = ArgumentArity.ZeroOrOne
        };

        Add(directoryArgument);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var directory = parseResult.GetValue(directoryArgument);
            return await processor.PullAsync(directory, cancellationToken) ? 0 : 1;
        });
    }
}
