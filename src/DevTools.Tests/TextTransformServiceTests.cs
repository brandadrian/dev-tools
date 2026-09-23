using DevTools.TextTransformCommand.Models;
using DevTools.TextTransformCommand.Services;
using Xunit;

namespace DevTools.Tests;

public class TextTransformServiceTests
{
    [Theory]
    [InlineData("Hello world", TextTransformType.Upper, "HELLO WORLD")]
    [InlineData("HELLO WORLD", TextTransformType.Lower, "hello world")]
    [InlineData("", TextTransformType.Upper, "")]
    [InlineData("", TextTransformType.Lower, "")]
    public async Task TransformAsync_ReturnsExpectedText(string text, TextTransformType transformType, string expected)
    {
        var service = new TextTransformService();
        var request = new TextTransformRequest
        {
            Text = text,
            TransformType = transformType
        };

        var result = await service.TransformAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(expected, result.TransformedText);
        Assert.Equal(text, result.OriginalText);
        Assert.Equal(transformType, result.TransformType);
        Assert.Null(result.ErrorMessage);
    }
}