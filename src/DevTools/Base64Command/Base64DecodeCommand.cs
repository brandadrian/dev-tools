using System.CommandLine;

namespace DevTools.Base64Command;

public class Base64DecodeCommand : Command
{
    public Base64DecodeCommand(Base64CommandProcessor processor) : base("base64-decode", "Decodes Base64 text.")
    {
        var textArg = new Argument<string>("text")
        {
            Description = "Base64 text to decode"
        };

        Add(textArg);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var text = parseResult.GetValue(textArg);
            return await processor.ExecuteDecode(text!, cancellationToken) ? 0 : 1;
        });
    }
}
