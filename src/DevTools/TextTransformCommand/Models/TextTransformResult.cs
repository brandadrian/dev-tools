namespace DevTools.TextTransformCommand.Models;

public class TextTransformResult
{
    public string OriginalText { get; set; } = string.Empty;
    public string TransformedText { get; set; } = string.Empty;
    public TextTransformType TransformType { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
