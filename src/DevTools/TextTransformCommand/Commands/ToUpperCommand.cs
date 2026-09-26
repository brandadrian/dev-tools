using System.CommandLine;

namespace DevTools.TextTransformCommand.Commands;

public class ToUpperCommand : Command
{
    public ToUpperCommand(TextTransformCommandProcessor processor) : base("to-upper", "Converts text to uppercase.")
    {
        var textArg = new Argument<string>("text")
        {
            Description = "Text to convert to uppercase"
        };

        Add(textArg);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var text = parseResult.GetValue(textArg);
            return await processor.ExecuteToUpper(text!, cancellationToken) ? 0 : 1;
        });
    }
}
