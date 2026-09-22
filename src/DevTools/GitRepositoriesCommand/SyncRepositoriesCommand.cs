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

        Add(configPathArgument);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var configPath = parseResult.GetValue(configPathArgument);
            return await processor.SyncAsync(configPath!, cancellationToken) ? 0 : 1;
        });
    }
}
