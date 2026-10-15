using System.CommandLine;
using DevTools.GitRepositoriesCommand.commands;
using DevTools.GitRepositoriesCommand.Services;
using DevTools.GitRepositoriesCommand.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DevTools.GitRepositoriesCommand;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitRepositories(this IServiceCollection services)
    {
        return services
            .AddSingleton<IGitRepositorySyncService, GitRepositorySyncService>()
            .AddSingleton<IGitRepositoriesPullService, GitRepositoriesPullService>()
            .AddSingleton<GitRepositoriesCommandProcessor>()
            .AddSingleton<Command>(provider => provider.GetRequiredService<SyncRepositoriesCommand>())
            .AddSingleton<Command>(provider => provider.GetRequiredService<SyncMainBranchesCommand>())
            .AddSingleton<SyncRepositoriesCommand>()
            .AddSingleton<SyncMainBranchesCommand>();
    }
}
