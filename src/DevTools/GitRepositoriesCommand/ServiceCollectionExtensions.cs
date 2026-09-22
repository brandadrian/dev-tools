using System.CommandLine;
using DevTools.GitRepositoriesCommand.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevTools.GitRepositoriesCommand;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitRepositories(this IServiceCollection services)
    {
        return services
            .AddSingleton<IGitRepositorySyncService, GitRepositorySyncService>()
            .AddSingleton<GitRepositoriesCommandProcessor>()
            .AddSingleton<Command, SyncRepositoriesCommand>();
    }
}
