using DevTools.Extensions;
using Xunit;

namespace DevTools.Tests.Extensions;

public class EnumerableExtensionsTests
{
    [Fact]
    public void Partition_SplitsItems_ByPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6 };

        // Act
        var (evens, odds) = source.Partition(n => n % 2 == 0);

        // Assert
        Assert.Equal([2, 4, 6], evens);
        Assert.Equal([1, 3, 5], odds);
    }

    [Fact]
    public void Partition_EmptySource_ReturnsTwoEmptyLists()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act
        var (trueList, falseList) = source.Partition(n => n > 0);

        // Assert
        Assert.Empty(trueList);
        Assert.Empty(falseList);
    }

    [Fact]
    public void Partition_AllMatch_ReturnsEmptyFalseList()
    {
        // Arrange
        var source = new[] { 2, 4, 6 };

        // Act
        var (trueList, falseList) = source.Partition(n => n % 2 == 0);

        // Assert
        Assert.Equal(source, trueList);
        Assert.Empty(falseList);
    }

    [Fact]
    public void Partition_NoneMatch_ReturnsEmptyTrueList()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };

        // Act
        var (trueList, falseList) = source.Partition(n => n % 2 == 0);

        // Assert
        Assert.Empty(trueList);
        Assert.Equal(source, falseList);
    }

    [Fact]
    public void Partition_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => source!.Partition(n => n > 0));
    }

    [Fact]
    public void Partition_NullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => source.Partition(null!));
    }
}
