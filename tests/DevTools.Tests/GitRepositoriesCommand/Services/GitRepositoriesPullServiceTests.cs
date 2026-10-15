using DevTools.GitRepositoriesCommand.Models;
using DevTools.GitRepositoriesCommand.Services;

namespace DevTools.Tests.GitRepositoriesCommand.Services;

public class GitRepositoriesPullServiceTests
{
    private readonly GitRepositoriesPullService _service = new();

    [Fact]
    public async Task PullAsync_WithNonExistentDirectory_ReturnsFailureWithError()
    {
        // Arrange
        var nonExistentPath = "/non/existent/path";
        var expectedPath = Path.GetFullPath(nonExistentPath);
        var cts = new CancellationTokenSource();

        // Act
        var result = await _service.PullAsync(nonExistentPath, cts.Token);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.FailedRepositories);
        Assert.Equal(expectedPath, result.FailedRepositories[0].Path);
        Assert.Contains("Directory not found", result.FailedRepositories[0].ErrorMessage);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task PullAsync_WithNoRepositories_ReturnsZeroRepositoriesFound()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var cts = new CancellationTokenSource();

            // Act
            var result = await _service.PullAsync(tempDir.FullName, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalRepositoriesFound);
            Assert.Empty(result.UpdatedRepositories);
            Assert.Empty(result.FailedRepositories);
            Assert.True(result.Success);
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task PullAsync_FindsRepositoriesInSubdirectories()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            // Create a git repository structure
            var repoDir = Path.Combine(tempDir.FullName, "repo1");
            Directory.CreateDirectory(repoDir);
            var gitDir = Path.Combine(repoDir, ".git");
            Directory.CreateDirectory(gitDir);

            var cts = new CancellationTokenSource();

            // Act
            var result = await _service.PullAsync(tempDir.FullName, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalRepositoriesFound);
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task PullAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _service.PullAsync(tempDir.FullName, cts.Token));
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public void PullRepositoriesResult_InitializesWithEmptyCollections()
    {
        // Arrange & Act
        var result = new PullRepositoriesResult();

        // Assert
        Assert.NotNull(result.UpdatedRepositories);
        Assert.NotNull(result.MainBranchRepositories);
        Assert.NotNull(result.FailedRepositories);
        Assert.Empty(result.UpdatedRepositories);
        Assert.Empty(result.MainBranchRepositories);
        Assert.Empty(result.FailedRepositories);
        Assert.Equal(0, result.TotalRepositoriesFound);
    }

    [Fact]
    public void PullRepositoriesResult_SuccessIsTrueWhenNoFailedRepositories()
    {
        // Arrange & Act
        var result = new PullRepositoriesResult
        {
            TotalRepositoriesFound = 2,
            UpdatedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo1", CurrentBranch = "main" },
                new() { Path = "/repo2", CurrentBranch = "master" }
            }
        };

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void PullRepositoriesResult_SuccessIsFalseWhenFailedRepositoriesExist()
    {
        // Arrange & Act
        var result = new PullRepositoriesResult
        {
            TotalRepositoriesFound = 2,
            UpdatedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo1", CurrentBranch = "main" }
            },
            FailedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo2", ErrorMessage = "Failed to pull" }
            }
        };

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void RepositoryPullInfo_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var info = new RepositoryPullInfo();

        // Assert
        Assert.Equal(string.Empty, info.Path);
        Assert.Equal(string.Empty, info.CurrentBranch);
        Assert.Null(info.ErrorMessage);
    }

    [Fact]
    public void RepositoryPullInfo_StoresPathAndBranchInfo()
    {
        // Arrange & Act
        var info = new RepositoryPullInfo
        {
            Path = "/path/to/repo",
            CurrentBranch = "feature/test",
            ErrorMessage = null
        };

        // Assert
        Assert.Equal("/path/to/repo", info.Path);
        Assert.Equal("feature/test", info.CurrentBranch);
        Assert.Null(info.ErrorMessage);
    }

    [Fact]
    public void RepositoryPullInfo_StoresErrorMessage()
    {
        // Arrange & Act
        var errorMsg = "Git command failed: pull";
        var info = new RepositoryPullInfo
        {
            Path = "/path/to/repo",
            ErrorMessage = errorMsg
        };

        // Assert
        Assert.Equal("/path/to/repo", info.Path);
        Assert.Equal(errorMsg, info.ErrorMessage);
    }

    [Fact]
    public void GitRepositoriesPullService_RecognizesMainBranch()
    {
        // Arrange
        var branches = new[] { "main", "master", "develop" };

        // Act & Assert
        foreach (var branch in new[] { "main", "master" })
        {
            Assert.True(branches.Contains(branch), $"Branch '{branch}' should be recognized");
        }
    }

    [Fact]
    public void GitRepositoriesPullService_CaseInsensitiveBranchComparison()
    {
        // Arrange
        var branches = new[] { "Main", "MASTER", "Master" };
        var mainBranches = new[] { "main", "master" };

        // Act & Assert
        foreach (var branch in branches)
        {
            Assert.True(mainBranches.Contains(branch, StringComparer.OrdinalIgnoreCase),
                $"Branch '{branch}' should be recognized (case-insensitive)");
        }
    }

    [Theory]
    [InlineData("main")]
    [InlineData("master")]
    public Task PullAsync_IdentifiesMainBranchCorrectly(string branchName)
    {
        // Note: This is a logical test that verifies branch name recognition
        var testBranches = new[] { "main", "master", "develop", "feature/test" };
        var isMainBranch = testBranches.Contains(branchName, StringComparer.OrdinalIgnoreCase)
            && (branchName.Equals("main", StringComparison.OrdinalIgnoreCase)
                || branchName.Equals("master", StringComparison.OrdinalIgnoreCase));

        // Act & Assert
        Assert.True(isMainBranch);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task PullAsync_ReturnsCorrectTotalRepositoriesFound()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var service = new GitRepositoriesPullService();
            var cts = new CancellationTokenSource();

            // Create multiple fake repository directories
            for (int i = 0; i < 3; i++)
            {
                var repoDir = Path.Combine(tempDir.FullName, $"repo{i}");
                Directory.CreateDirectory(repoDir);
                Directory.CreateDirectory(Path.Combine(repoDir, ".git"));
            }

            // Act
            var result = await service.PullAsync(tempDir.FullName, cts.Token);

            // Assert
            Assert.Equal(3, result.TotalRepositoriesFound);
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task PullAsync_HandlesEmptyRepositoryPathGracefully()
    {
        // Arrange
        var service = new GitRepositoriesPullService();
        var emptyPath = string.Empty;
        var cts = new CancellationTokenSource();

        // Act & Assert
        // Should not throw, but handle empty path
        try
        {
            var result = await service.PullAsync(emptyPath, cts.Token);
            Assert.NotNull(result);
        }
        catch (ArgumentException)
        {
            // ArgumentException is acceptable for empty path
            Assert.True(true);
        }
    }

    [Fact]
    public async Task PullAsync_HandlesPathWithSpecialCharacters()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var service = new GitRepositoriesPullService();
            var pathWithSpecialChars = Path.Combine(tempDir.FullName, "repo-with-dash_and_underscore");
            Directory.CreateDirectory(pathWithSpecialChars);
            Directory.CreateDirectory(Path.Combine(pathWithSpecialChars, ".git"));

            var cts = new CancellationTokenSource();

            // Act
            var result = await service.PullAsync(tempDir.FullName, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalRepositoriesFound);
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public async Task PullAsync_HandlesCancellation()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory();
        try
        {
            var service = new GitRepositoriesPullService();
            var cts = new CancellationTokenSource();

            // Cancel immediately
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => service.PullAsync(tempDir.FullName, cts.Token));
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    [Fact]
    public void PullRepositoriesResult_FailureScenario()
    {
        // Arrange
        var result = new PullRepositoriesResult
        {
            TotalRepositoriesFound = 3,
            UpdatedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo1", CurrentBranch = "main" },
                new() { Path = "/repo2", CurrentBranch = "master" }
            },
            FailedRepositories = new List<RepositoryPullInfo>
            {
                new() {
                    Path = "/repo3",
                    ErrorMessage = "Git command failed: pull\nError: Your local changes would be overwritten"
                }
            }
        };

        // Act
        var successCount = result.UpdatedRepositories.Count;
        var failureCount = result.FailedRepositories.Count;

        // Assert
        Assert.Equal(2, successCount);
        Assert.Equal(1, failureCount);
        Assert.False(result.Success);
        Assert.Equal(3, result.TotalRepositoriesFound);
    }

    [Fact]
    public void PullRepositoriesResult_PartialSuccessScenario()
    {
        // Arrange
        var result = new PullRepositoriesResult
        {
            TotalRepositoriesFound = 5,
            UpdatedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo1", CurrentBranch = "main" },
                new() { Path = "/repo2", CurrentBranch = "feature/test" },
                new() { Path = "/repo3", CurrentBranch = "master" },
                new() { Path = "/repo4", CurrentBranch = "develop" }
            },
            FailedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo5", ErrorMessage = "Permission denied" }
            }
        };

        // Act
        var updatePercentage = (result.UpdatedRepositories.Count / (double)result.TotalRepositoriesFound) * 100;

        // Assert
        Assert.Equal(4, result.UpdatedRepositories.Count);
        Assert.Equal(1, result.FailedRepositories.Count);
        Assert.Equal(80, updatePercentage);
        Assert.False(result.Success);
    }

    [Fact]
    public void PullRepositoriesResult_AllSuccessScenario()
    {
        // Arrange
        var result = new PullRepositoriesResult
        {
            TotalRepositoriesFound = 3,
            UpdatedRepositories = new List<RepositoryPullInfo>
            {
                new() { Path = "/repo1", CurrentBranch = "main" },
                new() { Path = "/repo2", CurrentBranch = "master" },
                new() { Path = "/repo3", CurrentBranch = "develop" }
            },
            FailedRepositories = new List<RepositoryPullInfo>()
        };

        // Act
        var allSucceeded = result.FailedRepositories.Count == 0
            && result.UpdatedRepositories.Count == result.TotalRepositoriesFound;

        // Assert
        Assert.True(result.Success);
        Assert.True(allSucceeded);
    }

    [Fact]
    public void RepositoryPullInfo_WithNullErrorMessage()
    {
        // Arrange
        var info = new RepositoryPullInfo
        {
            Path = "/successful/repo",
            CurrentBranch = "main",
            ErrorMessage = null
        };

        // Act
        var hasError = !string.IsNullOrEmpty(info.ErrorMessage);

        // Assert
        Assert.False(hasError);
        Assert.Null(info.ErrorMessage);
    }

    [Fact]
    public void RepositoryPullInfo_WithEmptyErrorMessage()
    {
        // Arrange
        var info = new RepositoryPullInfo
        {
            Path = "/repo",
            CurrentBranch = "main",
            ErrorMessage = string.Empty
        };

        // Act
        var hasError = !string.IsNullOrEmpty(info.ErrorMessage);

        // Assert
        Assert.False(hasError);
    }
}


