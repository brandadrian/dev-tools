using DevTools.IdGeneratorCommand;
using DevTools.IdGeneratorCommand.Models;
using DevTools.IdGeneratorCommand.Services;

namespace DevTools.Tests.IdGeneratorCommand;

public class IdGeneratorCommandProcessorTests
{
    [Fact]
    public async Task ExecuteUuidV4_Success_WritesIdAndReturnsTrue()
    {
        var stub = new StubIdGeneratorService(new IdResult
        {
            Success = true,
            Id = "b61211e8-88be-4cad-9a1e-916917d898fa"
        });

        var processor = new IdGeneratorCommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(
            () => processor.ExecuteUuidV4(withoutDashes: false, CancellationToken.None));

        Assert.True(executionResult);
        Assert.Equal("b61211e8-88be-4cad-9a1e-916917d898fa", consoleOutput);
        Assert.Equal(IdType.UuidV4, stub.LastRequest!.Type);
        Assert.False(stub.LastRequest.WithoutDashes);
    }

    [Fact]
    public async Task ExecuteUuidV4_WithoutDashes_ForwardsFlagToService()
    {
        var stub = new StubIdGeneratorService(new IdResult
        {
            Success = true,
            Id = "b61211e888be4cad9a1e916917d898fa"
        });

        var processor = new IdGeneratorCommandProcessor(stub);

        await processor.ExecuteUuidV4(withoutDashes: true, CancellationToken.None);

        Assert.True(stub.LastRequest!.WithoutDashes);
    }

    [Fact]
    public async Task ExecuteUuidV4_Failure_WritesErrorMessageAndReturnsFalse()
    {
        var stub = new StubIdGeneratorService(new IdResult
        {
            Success = false,
            ErrorMessage = "error"
        });

        var processor = new IdGeneratorCommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(
            () => processor.ExecuteUuidV4(withoutDashes: false, CancellationToken.None));

        Assert.False(executionResult);
        Assert.Equal("Error generating id: error", consoleOutput);
    }

    [Fact]
    public async Task ExecuteUuidV7_Success_ForwardsUuidV7Type()
    {
        var stub = new StubIdGeneratorService(new IdResult
        {
            Success = true,
            Id = "id"
        });

        var processor = new IdGeneratorCommandProcessor(stub);

        await processor.ExecuteUuidV7(withoutDashes: false, CancellationToken.None);

        Assert.Equal(IdType.UuidV7, stub.LastRequest!.Type);
    }

    [Fact]
    public async Task ExecuteSnowflake_Success_WritesIdAndReturnsTrue()
    {
        var stub = new StubIdGeneratorService(new IdResult
        {
            Success = true,
            Id = "2102825432419143680"
        });

        var processor = new IdGeneratorCommandProcessor(stub);

        var (executionResult, consoleOutput) = await CaptureConsoleOutput(
            () => processor.ExecuteSnowflake(CancellationToken.None));

        Assert.True(executionResult);
        Assert.Equal("2102825432419143680", consoleOutput);
        Assert.Equal(IdType.Snowflake, stub.LastRequest!.Type);
    }

    private sealed class StubIdGeneratorService(IdResult result) : IIdGeneratorService
    {
        public IdRequest? LastRequest { get; private set; }

        public ValueTask<IdResult> GenerateAsync(IdRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return new ValueTask<IdResult>(result);
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
