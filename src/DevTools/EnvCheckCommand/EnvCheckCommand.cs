namespace DevTools.EnvCheckCommand;

using System.CommandLine;

public class EnvCheckCommand : Command
{
    public EnvCheckCommand(EnvCheckCommandProcessor envCheckCommandProcessor) : base("env-check", "Check installed development tools and their versions")
    {
        SetAction(async (_, cancellationToken) => await envCheckCommandProcessor.ExecuteAsync(cancellationToken));
    }
}
