namespace DevTools.Base64Command;

using Models;
using Services;

public class Base64CommandProcessor(IBase64Service base64Service)
{
    public Task<bool> ExecuteEncode(string text, CancellationToken cancellationToken) =>
        ExecuteAsync(text, Base64OperationType.Encode, cancellationToken);

    public Task<bool> ExecuteDecode(string text, CancellationToken cancellationToken) =>
        ExecuteAsync(text, Base64OperationType.Decode, cancellationToken);

    private async Task<bool> ExecuteAsync(string text, Base64OperationType operationType, CancellationToken cancellationToken)
    {
        var request = new Base64Request
        {
            Text = text,
            OperationType = operationType
        };

        var result = await base64Service.TransformAsync(request, cancellationToken);

        if (result.Success)
        {
            Console.WriteLine(result.TransformedText);
            return true;
        }

        var action = operationType == Base64OperationType.Encode ? "encoding" : "decoding";
        Console.WriteLine($"Error {action} text: {result.ErrorMessage}");
        return false;
    }
}

