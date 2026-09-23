using DevTools.Base64Command.Models;
using DevTools.Base64Command.Services;

namespace DevTools.Tests.Base64Command.Services;

public class Base64ServiceTests
{
    private readonly Base64Service _sut = new();

    [Theory]
    [InlineData("username:password", "dXNlcm5hbWU6cGFzc3dvcmQ=")]
    [InlineData("test1 test2", "dGVzdDEgdGVzdDI=")]
    [InlineData("", "")]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit", "TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2NpbmcgZWxpdA==")]
    public async Task TransformAsync_Encode_ReturnsBase64EncodedText(string text, string expectedBase64)
    {
        var request = new Base64Request
        {
            Text = text, 
            OperationType = Base64OperationType.Encode
        };

        var result = await _sut.TransformAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(expectedBase64, result.TransformedText);
        Assert.Equal(Base64OperationType.Encode, result.OperationType);
        Assert.Null(result.ErrorMessage);
    }

    [Theory]
    [InlineData("dXNlcm5hbWU6cGFzc3dvcmQ=", "username:password")]
    [InlineData("dGVzdDEgdGVzdDI=", "test1 test2")]
    [InlineData("", "")]
    [InlineData("TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2NpbmcgZWxpdA==", "Lorem ipsum dolor sit amet, consectetur adipiscing elit")]
    public async Task TransformAsync_Decode_ReturnsOriginalText(string base64Text, string expectedText)
    {
        var request = new Base64Request
        {
            Text = base64Text, 
            OperationType = Base64OperationType.Decode
        };

        var result = await _sut.TransformAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(expectedText, result.TransformedText);
        Assert.Equal(Base64OperationType.Decode, result.OperationType);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task TransformAsync_Decode_InvalidBase64_ReturnsFailureResult()
    {
        var request = new Base64Request
        {
            Text = "!!not-valid-base64!!", 
            OperationType = Base64OperationType.Decode
        };

        var result = await _sut.TransformAsync(request, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.TransformedText);
        Assert.NotNull(result.ErrorMessage);
    }


    [Fact]
    public async Task TransformAsync_UnknownOperationType_ReturnsFailureResult()
    {
        var request = new Base64Request
        {
            Text = "text", 
            OperationType = (Base64OperationType)99
        };

        var result = await _sut.TransformAsync(request, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("Unknown operation type", result.ErrorMessage);
    }

    [Fact]
    public async Task TransformAsync_EncodeThenDecode_RoundTripsOriginalText()
    {
        const string original = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";

        var encodeResult = await _sut.TransformAsync(
            new Base64Request
            {
                Text = original, 
                OperationType = Base64OperationType.Encode
            },
            CancellationToken.None);

        var decodeResult = await _sut.TransformAsync(
            new Base64Request
            {
                Text = encodeResult.TransformedText, 
                OperationType = Base64OperationType.Decode
            },
            CancellationToken.None);

        Assert.Equal(original, decodeResult.TransformedText);
    }
}
