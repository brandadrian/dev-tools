using DevTools.TextTransformCommand.Commands;
using DevTools.TextTransformCommand.Services;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

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
