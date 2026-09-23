using System.CommandLine;

namespace DevTools.Base64Command;

public class Base64EncodeCommand : Command
{
    public Base64EncodeCommand(Base64CommandProcessor processor) : base("base64-encode", "Encodes text to Base64.")
    {
        var textArg = new Argument<string>("text")
        {
            Description = "Text to encode to Base64"
        };

        Add(textArg);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var text = parseResult.GetValue(textArg);
            return await processor.ExecuteEncode(text!, cancellationToken) ? 0 : 1;
        });
    }
}
