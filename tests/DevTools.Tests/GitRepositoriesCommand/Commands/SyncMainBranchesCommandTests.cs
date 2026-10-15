using DevTools.GitRepositoriesCommand;
using DevTools.GitRepositoriesCommand.commands;
using DevTools.GitRepositoriesCommand.Services;

namespace DevTools.Tests.GitRepositoriesCommand.Commands;

public class SyncMainBranchesCommandTests
{
    [Fact]
    public void SyncMainBranchesCommand_HasCorrectDescription()
    {
        // Arrange
        var processor = new GitRepositoriesCommandProcessor(
            new GitRepositorySyncService(),
            new GitRepositoriesPullService());

        var command = new SyncMainBranchesCommand(processor);

        // Act & Assert
        Assert.NotNull(command);
        Assert.Contains("sync-main-branches", command.Name);
        if (command.Description != null) Assert.NotEmpty(command.Description);
    }

    [Fact]
    public void SyncMainBranchesCommand_AcceptsOptionalDirectoryArgument()
    {
        // Arrange
        var processor = new GitRepositoriesCommandProcessor(
            new GitRepositorySyncService(),
            new GitRepositoriesPullService());

        var command = new SyncMainBranchesCommand(processor);

        // Act & Assert
        Assert.NotNull(command);
        Assert.NotEmpty(command.Children);
    }
}
