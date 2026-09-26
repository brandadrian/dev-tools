using System.CommandLine;

namespace DevTools.IdGeneratorCommand.Commands;

public class SnowflakeCommand : Command
{
    public SnowflakeCommand(IdGeneratorCommandProcessor processor) : base("snowflake", "Generates a Twitter Snowflake id.")
    {
        SetAction(async (_, cancellationToken) => await processor.ExecuteSnowflake(cancellationToken) ? 0 : 1);
    }
}
