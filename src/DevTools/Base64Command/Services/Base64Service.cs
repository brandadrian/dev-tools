using DevTools.Base64Command.Models;

namespace DevTools.Base64Command.Services;

public class Base64Service : IBase64Service
{
    public ValueTask<Base64Result> TransformAsync(Base64Request request, CancellationToken cancellationToken)
    {
        try
        {
            var transformedText = request.OperationType switch
            {
                Base64OperationType.Encode => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Text)),
                Base64OperationType.Decode => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Text)),
                _ => throw new InvalidOperationException($"Unknown operation type: {request.OperationType}")
            };

            return new ValueTask<Base64Result>(new Base64Result
            {
                TransformedText = transformedText,
                OperationType = request.OperationType,
                Success = true
            });
        }
        catch (Exception ex)
        {
            return new ValueTask<Base64Result>(new Base64Result
            {
                TransformedText = string.Empty,
                OperationType = request.OperationType,
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }
}
