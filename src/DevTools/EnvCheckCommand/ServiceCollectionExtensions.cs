namespace DevTools.EnvCheckCommand;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnvCheck(this IServiceCollection services)
    {
        return services
            .AddSingleton<IEnvCheckService, EnvCheckService>()
            .AddSingleton<EnvCheckCommandProcessor>()
            .AddSingleton<Command, EnvCheckCommand>();
    }
}
