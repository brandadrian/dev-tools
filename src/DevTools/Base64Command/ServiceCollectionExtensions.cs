using DevTools.Base64Command.Services;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace DevTools.Base64Command;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBase64(this IServiceCollection services)
    {
        return services
            .AddSingleton<IBase64Service, Base64Service>()
            .AddSingleton<Base64CommandProcessor>()
            .AddSingleton<Command, Base64EncodeCommand>()
            .AddSingleton<Command, Base64DecodeCommand>();
    }
}
