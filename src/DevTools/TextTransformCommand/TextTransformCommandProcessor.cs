namespace DevTools.TextTransformCommand;

using DevTools.TextTransformCommand.Models;
using DevTools.TextTransformCommand.Services;

public class TextTransformCommandProcessor
{
    private readonly ITextTransformService _textTransformService;

    public TextTransformCommandProcessor(ITextTransformService textTransformService)
    {
        _textTransformService = textTransformService;
    }

    public async Task<bool> ExecuteToUpper(string text, CancellationToken cancellationToken)
    {
        var request = new TextTransformRequest
        {
            Text = text,
            TransformType = TextTransformType.Upper
        };

        var result = await _textTransformService.TransformAsync(request, cancellationToken);
        
        if (result.Success)
        {
            Console.WriteLine(result.TransformedText);
            return true;
        }
        
        Console.WriteLine($"Error transforming text: {result.ErrorMessage}");
        return false;
    }

    public async Task<bool> ExecuteToLower(string text, CancellationToken cancellationToken)
    {
        var request = new TextTransformRequest
        {
            Text = text,
            TransformType = TextTransformType.Lower
        };

        var result = await _textTransformService.TransformAsync(request, cancellationToken);
        
        if (result.Success)
        {
            Console.WriteLine(result.TransformedText);
            return true;
        }
        
        Console.WriteLine($"Error transforming text: {result.ErrorMessage}");
        return false;
    }
}
