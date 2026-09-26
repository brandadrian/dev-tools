using System.CommandLine;

namespace DevTools.IdGeneratorCommand.Commands;

public class UuidV7Command : Command
{
    public UuidV7Command(IdGeneratorCommandProcessor processor) : base("uuid-v7", "Generates a time-ordered UUID (version 7).")
    {
        var noDashesOption = new Option<bool>("--no-dashes")
        {
            Description = "Omit dashes from the generated id"
        };

        Add(noDashesOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var withoutDashes = parseResult.GetValue(noDashesOption);
            return await processor.ExecuteUuidV7(withoutDashes, cancellationToken) ? 0 : 1;
        });
    }
}
