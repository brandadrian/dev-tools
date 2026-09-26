using DevTools.IdGeneratorCommand.Services;

namespace DevTools.Tests.IdGeneratorCommand.Services;

public class SnowflakeIdGeneratorTests
{
    [Fact]
    public void NextId_ReturnsPositiveValue()
    {
        var sut = new SnowflakeIdGenerator();

        Assert.True(sut.NextId() > 0);
    }

    [Fact]
    public void NextId_SuccessiveCalls_ReturnUniqueIncreasingValues()
    {
        var sut = new SnowflakeIdGenerator();

        var ids = Enumerable.Range(0, 1000).Select(_ => sut.NextId()).ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
        Assert.Equal(ids, ids.OrderBy(id => id));
    }

    [Fact]
    public void NextId_ConcurrentCalls_ReturnUniqueValues()
    {
        var sut = new SnowflakeIdGenerator();

        var ids = Enumerable.Range(0, 8)
            .AsParallel()
            .SelectMany(_ => Enumerable.Range(0, 500).Select(_ => sut.NextId()))
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1024)]
    public void Constructor_MachineIdOutOfRange_ThrowsArgumentOutOfRangeException(long machineId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SnowflakeIdGenerator(machineId));
    }
}
