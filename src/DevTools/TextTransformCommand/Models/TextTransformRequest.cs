namespace DevTools.TextTransformCommand.Models;

public class TextTransformRequest
{
    public string Text { get; set; } = string.Empty;
    public TextTransformType TransformType { get; set; }
}

public enum TextTransformType
{
    Upper,
    Lower
}
