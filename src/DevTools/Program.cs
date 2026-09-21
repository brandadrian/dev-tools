using DevTools;
using DevTools.TextTransformCommand;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddTextTransform()
    .AddSingleton<Cli>();

var provider = services.BuildServiceProvider();
var cli = provider.GetRequiredService<Cli>();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Cancelling...");
    cts.Cancel();
    e.Cancel = true;
};

return await cli.ExecuteAsync(args, cts.Token);