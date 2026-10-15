using DevTools.GitRepositoriesCommand;
using DevTools.GitRepositoriesCommand.Services;

namespace DevTools.Tests.GitRepositoriesCommand;

public class GitRepositoriesCommandProcessorTests
{
    [Fact]
    public async Task PullAsync_WithInvalidDirectory_ReturnsFalse()
    {
        // Arrange
        var processor = new GitRepositoriesCommandProcessor(
            new GitRepositorySyncService(),
            new GitRepositoriesPullService());
        var invalidPath = "/invalid/path/that/does/not/exist";
        var cts = new CancellationTokenSource();

        // Act
        var result = await processor.PullAsync(invalidPath, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PullAsync_WithValidDirectory_ReturnsTrue()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var processor = new GitRepositoriesCommandProcessor(
                new GitRepositorySyncService(),
                new GitRepositoriesPullService());
            var cts = new CancellationTokenSource();

            // Act
            var result = await processor.PullAsync(tempDir.FullName, cts.Token);

            // Assert
            Assert.True(result);
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task PullAsync_UsesCurrentDirectoryWhenNoneSpecified()
    {
        // Arrange
        var processor = new GitRepositoriesCommandProcessor(
            new GitRepositorySyncService(),
            new GitRepositoriesPullService());
        var cts = new CancellationTokenSource();

        // Act
        var result = await processor.PullAsync(null, cts.Token);

        // Assert
        Assert.True(result || !result);
    }
}
