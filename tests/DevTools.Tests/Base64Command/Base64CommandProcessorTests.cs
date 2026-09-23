using DevTools.Base64Command;
using DevTools.Base64Command.Models;
using DevTools.Base64Command.Services;

namespace DevTools.Tests.Base64Command;

public class Base64CommandProcessorTests
{
    [Fact]
    public async Task ExecuteEncode_Success_WritesTransformedTextAndReturnsTrue()
    {
        var stub = new StubBase64Service(new Base64Result
        {
            Success = true,
            TransformedText = "SGVsbG8="
        });

        var processor = new Base64CommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(() => processor.ExecuteEncode("Hello", CancellationToken.None));

        Assert.True(executionResult);
        Assert.Equal("SGVsbG8=", consoleOutput);
        Assert.Equal(Base64OperationType.Encode, stub.LastRequest!.OperationType);
        Assert.Equal("Hello", stub.LastRequest.Text);
    }

    [Fact]
    public async Task ExecuteEncode_Failure_WritesErrorMessageAndReturnsFalse()
    {
        var stub = new StubBase64Service(new Base64Result
        {
            Success = false,
            ErrorMessage = "failed"
        });

        var processor = new Base64CommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(() => processor.ExecuteEncode("Hello", CancellationToken.None));

        Assert.False(executionResult);
        Assert.Equal("Error encoding text: failed", consoleOutput);
    }

    [Fact]
    public async Task ExecuteDecode_Success_WritesTransformedTextAndReturnsTrue()
    {
        var stub = new StubBase64Service(new Base64Result
        {
            Success = true,
            TransformedText = "Hello"
        });

        var processor = new Base64CommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(() => processor.ExecuteDecode("SGVsbG8=", CancellationToken.None));

        Assert.True(executionResult);
        Assert.Equal("Hello", consoleOutput);
        Assert.Equal(Base64OperationType.Decode, stub.LastRequest!.OperationType);
        Assert.Equal("SGVsbG8=", stub.LastRequest.Text);
    }

    [Fact]
    public async Task ExecuteDecode_Failure_WritesErrorMessageAndReturnsFalse()
    {
        var stub = new StubBase64Service(new Base64Result
        {
            Success = false,
            ErrorMessage = "failed"
        });

        var processor = new Base64CommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(() => processor.ExecuteDecode("not-valid", CancellationToken.None));

        Assert.False(executionResult);
        Assert.Equal("Error decoding text: failed", consoleOutput);
    }

    private sealed class StubBase64Service(Base64Result result) : IBase64Service
    {
        public Base64Request? LastRequest { get; private set; }

        public ValueTask<Base64Result> TransformAsync(Base64Request request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return new ValueTask<Base64Result>(result);
        }
    }

    private static async Task<(bool ExecutionResult, string ConsoleOutput)> CaptureConsoleOutput(Func<Task<bool>> action)
    {
        var originalOut = Console.Out;
        await using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var executionResult = await action();
            return (executionResult, writer.ToString().TrimEnd());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
