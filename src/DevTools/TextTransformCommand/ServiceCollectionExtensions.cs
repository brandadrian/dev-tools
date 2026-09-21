using System.CommandLine;
using DevTools.TextTransformCommand.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevTools.TextTransformCommand;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextTransform(this IServiceCollection services)
    {
        return services
            .AddSingleton<ITextTransformService, TextTransformService>()
            .AddSingleton<TextTransformCommandProcessor>()
            .AddSingleton<Command, ToUpperCommand>()
            .AddSingleton<Command, ToLowerCommand>();
    }
}