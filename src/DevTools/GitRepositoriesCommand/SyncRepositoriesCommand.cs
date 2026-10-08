using System.CommandLine;
using DevTools.GitRepositoriesCommand.Services;

namespace DevTools.GitRepositoriesCommand;

public class SyncRepositoriesCommand : Command
{
    public SyncRepositoriesCommand(GitRepositoriesCommandProcessor processor)
        : base("sync-repos", "Clones missing repositories and pulls existing ones from a JSON config file.")
    {
        var configPathArgument = new Argument<string>("config-path")
        {
            Description = "Path to the JSON file that defines repositories."
        };

        var baseDirectoryOption = new Option<string?>("--base-directory")
        {
            Description = "Base directory for relative repository folders. Defaults to the current directory."
        };

        Add(configPathArgument);
        Add(baseDirectoryOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var configPath = parseResult.GetValue(configPathArgument);
            var baseDirectory = parseResult.GetValue(baseDirectoryOption);
            return await processor.SyncAsync(configPath!, baseDirectory, cancellationToken) ? 0 : 1;
        });
    }
}
