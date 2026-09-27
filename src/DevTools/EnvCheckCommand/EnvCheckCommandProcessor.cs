using DevTools.EnvCheckCommand.Services;
using DevTools.Extensions;

namespace DevTools.EnvCheckCommand;

public class EnvCheckCommandProcessor(IEnvCheckService envCheckService)
{
    private static readonly int ToolNameWidth = 15;
    private static readonly string ToolNamePadding = new(' ', ToolNameWidth);

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        var toolsList = (await envCheckService.CheckEnvironmentAsync(cancellationToken)).ToList();

        var (installedList, missingList) = toolsList.Partition(t => t.IsInstalled);

        if (installedList.Count > 0)
        {
            PrintColoredHeader("✓ Installed Tools:", ConsoleColor.Green);

            foreach (var tool in installedList)
            {
                var versionLines = tool.Version?.Split('\n') ?? [""];

                Console.WriteLine($"  {tool.Name.PadRight(ToolNameWidth)} -> {versionLines[0]}");

                for (var i = 1; i < versionLines.Length; i++)
                {
                    Console.WriteLine($"  {ToolNamePadding}    {versionLines[i]}");
                }
            }
        }

        if (missingList.Count > 0)
        {
            PrintColoredHeader("X Missing Tools:", ConsoleColor.Yellow);

            foreach (var tool in missingList)
            {
                Console.WriteLine($"  {tool.Name.PadRight(ToolNameWidth)} -> Not found");
            }
        }

        var installCount = installedList.Count;
        var totalCount = toolsList.Count;
        Console.WriteLine($"\n Summary: {installCount}/{totalCount} tools installed\n");

        return 0;
    }

    private static void PrintColoredHeader(string text, ConsoleColor color)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
