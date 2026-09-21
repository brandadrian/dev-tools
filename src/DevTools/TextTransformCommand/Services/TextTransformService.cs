using DevTools.TextTransformCommand.Models;

namespace DevTools.TextTransformCommand.Services;

public interface ITextTransformService
{
    Task<TextTransformResult> TransformAsync(TextTransformRequest request, CancellationToken cancellationToken);
}

public class TextTransformService : ITextTransformService
{
    public async Task<TextTransformResult> TransformAsync(TextTransformRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var transformedText = request.TransformType switch
            {
                TextTransformType.Upper => request.Text.ToUpper(),
                TextTransformType.Lower => request.Text.ToLower(),
                _ => throw new InvalidOperationException($"Unknown transform type: {request.TransformType}")
            };

            return new TextTransformResult
            {
                OriginalText = request.Text,
                TransformedText = transformedText,
                TransformType = request.TransformType,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new TextTransformResult
            {
                OriginalText = request.Text,
                TransformedText = string.Empty,
                TransformType = request.TransformType,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
