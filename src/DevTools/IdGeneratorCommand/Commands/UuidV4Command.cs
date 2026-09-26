using System.CommandLine;

namespace DevTools.IdGeneratorCommand.Commands;

public class UuidV4Command : Command
{
    public UuidV4Command(IdGeneratorCommandProcessor processor) : base("uuid-v4", "Generates a random UUID (version 4).")
    {
        var noDashesOption = new Option<bool>("--no-dashes")
        {
            Description = "Omit dashes from the generated id"
        };

        Add(noDashesOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var withoutDashes = parseResult.GetValue(noDashesOption);
            return await processor.ExecuteUuidV4(withoutDashes, cancellationToken) ? 0 : 1;
        });
    }
}
