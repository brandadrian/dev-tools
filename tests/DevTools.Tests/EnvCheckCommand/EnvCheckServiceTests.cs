using DevTools.EnvCheckCommand.Services;
using Xunit;

namespace DevTools.Tests.EnvCheckCommand;

public class EnvCheckServiceTests
{
    private static readonly string[] ExpectedToolNames =
    [
        // Version Control
        "Git",
        // Core Development
        ".NET", "Java", "Python", "Node.js", "npm",
        // Frontend
        "TypeScript", "Yarn", "React CLI", "Angular CLI", "Vue CLI",
        // Backend
        "Go", "Rust", "Ruby", "PHP", "Gradle", "Maven",
        // DevOps & Infrastructure
        "Docker", "Podman", "Docker Compose",
        // Kubernetes & Orchestration
        "Kubernetes (kubectl)", "Kubectx", "Helm", "Kustomize", "Skaffold", "Kind", "Minikube",
        // Cloud & Infrastructure
        "Terraform", "AWS CLI", "Azure CLI", "GCloud CLI",
        // Database
        "PostgreSQL", "MySQL", "MongoDB", "Redis", "SQLite",
    ];

    [Fact]
    public async Task CheckEnvironmentAsync_ReturnsList_OfToolInfo()
    {
        // Arrange
        var service = new EnvCheckService();

        // Act
        var result = await service.CheckEnvironmentAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, tool =>
        {
            Assert.NotNull(tool.Name);
            Assert.NotNull(tool.Command);
        });
    }

    [Fact]
    public async Task CheckEnvironmentAsync_IncludesAllExpectedTools_WithExpectedCount()
    {
        // Arrange
        var service = new EnvCheckService();

        // Act
        var result = await service.CheckEnvironmentAsync();
        var names = result.Select(t => t.Name).ToList();

        // Assert
        Assert.Equal(ExpectedToolNames.Length, names.Count);
        Assert.All(ExpectedToolNames, expectedName => Assert.Contains(expectedName, names));
    }

    [Fact]
    public async Task CheckEnvironmentAsync_HasNoDuplicateToolNames()
    {
        // Arrange
        var service = new EnvCheckService();

        // Act
        var result = await service.CheckEnvironmentAsync();
        var names = result.Select(t => t.Name).ToList();

        // Assert
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public async Task CheckEnvironmentAsync_MissingTool_HasErrorAndNoVersion()
    {
        // Arrange
        var service = new EnvCheckService();

        // Act
        var result = await service.CheckEnvironmentAsync();
        var missingTools = result.Where(t => !t.IsInstalled).ToList();

        // Assert
        Assert.All(missingTools, tool =>
        {
            Assert.Null(tool.Version);
            Assert.NotNull(tool.Error);
        });
    }

    [Fact]
    public async Task CheckEnvironmentAsync_InstalledTool_HasVersionAndNoError()
    {
        // Arrange
        var service = new EnvCheckService();

        // Act
        var result = await service.CheckEnvironmentAsync();
        var installedTools = result.Where(t => t.IsInstalled).ToList();

        // Assert
        Assert.All(installedTools, tool =>
        {
            Assert.NotNull(tool.Version);
            Assert.Null(tool.Error);
        });
    }

    [Fact]
    public async Task CheckEnvironmentAsync_ThrowsWhenCancelledBeforeStart()
    {
        // Arrange
        var service = new EnvCheckService();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => service.CheckEnvironmentAsync(cts.Token));
    }
}
