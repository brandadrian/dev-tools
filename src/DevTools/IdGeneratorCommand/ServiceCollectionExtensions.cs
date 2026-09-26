using System.CommandLine;
using DevTools.IdGeneratorCommand.Commands;
using DevTools.IdGeneratorCommand.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevTools.IdGeneratorCommand;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdGenerator(this IServiceCollection services)
    {
        return services
            .AddSingleton<SnowflakeIdGenerator>()
            .AddSingleton<IIdGeneratorService, IdGeneratorService>()
            .AddSingleton<IdGeneratorCommandProcessor>()
            .AddSingleton<Command, UuidV4Command>()
            .AddSingleton<Command, UuidV7Command>()
            .AddSingleton<Command, SnowflakeCommand>();
    }
}
