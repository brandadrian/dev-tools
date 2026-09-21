using System.CommandLine;

namespace DevTools.TextTransformCommand;

public class ToLowerCommand : Command
{
    public ToLowerCommand(TextTransformCommandProcessor processor) : base("to-lower", "Converts text to lowercase.")
    {
        var textArg = new Argument<string>("text")
        {
            Description = "Text to convert to lowercase"
        };

        Add(textArg);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var text = parseResult.GetValue(textArg);
            return await processor.ExecuteToLower(text!, cancellationToken) ? 0 : 1;
        });
    }
}
