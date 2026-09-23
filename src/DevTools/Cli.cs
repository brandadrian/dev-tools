using System.CommandLine;

namespace DevTools;

public class Cli
{
    private readonly RootCommand _rootCommand;

    public Cli(IEnumerable<Command> commands)
    {
        _rootCommand = new RootCommand("DevTools is a command line tool for developers to perform various tasks and transformations on text and other data.");

        foreach (var cmd in commands)
        {
            _rootCommand.Add(cmd);
        }
    }

    public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var parseResult = _rootCommand.Parse(args);
        return await parseResult.InvokeAsync(cancellationToken: cancellationToken);
    }
}