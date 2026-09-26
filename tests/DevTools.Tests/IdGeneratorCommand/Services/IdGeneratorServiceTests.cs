using DevTools.IdGeneratorCommand.Models;
using DevTools.IdGeneratorCommand.Services;
using System.Text.RegularExpressions;

namespace DevTools.Tests.IdGeneratorCommand.Services;

public class IdGeneratorServiceTests
{
    private static readonly Regex GuidWithoutDashesPattern = new("^[0-9a-f]{32}$");
    private static readonly Regex UuidV4Pattern = new("^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$");
    private static readonly Regex UuidV7Pattern = new("^[0-9a-f]{8}-[0-9a-f]{4}-7[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$");

    private readonly IdGeneratorService _sut = new(new SnowflakeIdGenerator());

    [Fact]
    public async Task GenerateAsync_UuidV4_ReturnsVersion4Uuid()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = IdType.UuidV4
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Matches(UuidV4Pattern, result.Id);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task GenerateAsync_UuidV4_WithoutDashes_ReturnsIdWithoutDashes()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = IdType.UuidV4,
            WithoutDashes = true
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Matches(GuidWithoutDashesPattern, result.Id);
    }

    [Fact]
    public async Task GenerateAsync_UuidV7_ReturnsVersion7Uuid()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = IdType.UuidV7
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Matches(UuidV7Pattern, result.Id);
    }

    [Fact]
    public async Task GenerateAsync_UuidV7_WithoutDashes_ReturnsIdWithoutDashes()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = IdType.UuidV7,
            WithoutDashes = true
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Matches(GuidWithoutDashesPattern, result.Id);
    }

    [Fact]
    public async Task GenerateAsync_Snowflake_ReturnsPositiveInteger()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = IdType.Snowflake
        }, CancellationToken.None);

        Assert.True(result.Success);
        Assert.True(long.TryParse(result.Id, out var id));
        Assert.True(id > 0);
    }

    [Fact]
    public async Task GenerateAsync_UnknownType_ReturnsFailureResult()
    {
        var result = await _sut.GenerateAsync(new IdRequest
        {
            Type = (IdType)99
        }, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.Id);
        Assert.Contains("Unknown id type", result.ErrorMessage);
    }
}
